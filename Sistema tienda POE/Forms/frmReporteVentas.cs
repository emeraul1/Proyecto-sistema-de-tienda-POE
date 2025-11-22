using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Sistema_tienda_POE.Clases;
using Sistema_tienda_POE.UoW;
using System.Diagnostics;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace Sistema_tienda_POE.Forms
{
    public partial class frmReporteVentas : Form
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["Miconexion"].ConnectionString;
        public frmReporteVentas()
        {
            InitializeComponent();
            dtpInicio.Value = DateTime.Now.AddMonths(-1);
            dtpFin.Value = DateTime.Now;
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            ConfigurarDataGridView();
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            using (var uow = new UnitOfwork(_connectionString))
            {
                var listaCategorias = uow.Categoria.GetByEstado(true).ToList();

                listaCategorias.Add(new Categoria
                {
                    IdCategoria = 0,
                    Nombre = "Todas"
                });

                cmbCategoria.DataSource = null;
                cmbCategoria.DataSource = listaCategorias;
                cmbCategoria.DisplayMember = "Nombre";
                cmbCategoria.ValueMember = "IdCategoria";

                cmbCategoria.SelectedValue = 0;
                cmbCategoria.Refresh();
            }
        }

        private void ConfigurarDataGridView()
        {
            dgvReporte.AutoGenerateColumns = false;
            dgvReporte.Columns.Clear();

            dgvReporte.ReadOnly = true;
            dgvReporte.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReporte.AllowUserToAddRows = false;
            dgvReporte.MultiSelect = false;
            dgvReporte.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Producto
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Producto",
                HeaderText = "Producto",
                DataPropertyName = "Producto"
            });

            // Cantidad
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                HeaderText = "Cantidad",
                DataPropertyName = "Cantidad"
            });

            // Categoría
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Categoria",
                HeaderText = "Categoría",
                DataPropertyName = "Categoria"
            });

            // Precio Unitario
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioUnitario",
                HeaderText = "Precio Unitario",
                DataPropertyName = "PrecioUnitario",
                DefaultCellStyle = { Format = "C2" }
            });

            // Total
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Total",
                DataPropertyName = "Total",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            // Fecha
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                HeaderText = "Fecha",
                DataPropertyName = "Fecha",
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" }
            });

            // Vendido por (usuario)
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VendidoPor",
                HeaderText = "Vendido por",
                DataPropertyName = "VendidoPor"
            });

            // Método de pago
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MetodoPago",
                HeaderText = "Método de pago",
                DataPropertyName = "MetodoPago"
            });
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {

            // Validar fechas
            if (dtpInicio.Value.Date > dtpFin.Value.Date)
            {
                MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha fin.",
                                "Rango inválido",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Obtener categoría seleccionada
            int idCat = (int)cmbCategoria.SelectedValue;

            List<ReporteVenta> datos;

            using (var uow = new UnitOfwork(_connectionString))
            {
                int idCategoria = (int)cmbCategoria.SelectedValue;

                datos = uow.ReporteVenta.ObtenerReporteVentas(
                    dtpInicio.Value.Date,
                    dtpFin.Value.Date,
                    idCategoria
                );
            }

            dgvReporte.DataSource = null;
            dgvReporte.DataSource = datos;

            CalcularTotales(datos);
        }

        private void CalcularTotales(List<ReporteVenta> datos)
        {
            decimal totalVentas = datos.Sum(x => x.Total);
            decimal gananciaBruta = datos.Sum(x => (x.PrecioUnitario - x.CostoUnitario) * x.Cantidad);

            lbTotal.Text = $"Total ventas: {totalVentas:C2}";
            lbGanacia.Text = $"Ganancia bruta: {gananciaBruta:C2}";
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            // Validar que haya datos
            if (dgvReporte.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No hay datos para exportar. Primero genera un reporte.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            using (var guardar = new SaveFileDialog())
            {
                guardar.Filter = "Archivos PDF (*.pdf)|*.pdf";
                guardar.FileName = "Reporte_Ventas_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".pdf";

                if (guardar.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportarDataGridViewAPDF(guardar.FileName);

                        MessageBox.Show(
                            "PDF generado exitosamente en:\n" + guardar.FileName,
                            "Éxito",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        // Preguntar si desea abrirlo
                        var abrir = MessageBox.Show(
                            "¿Desea abrir el archivo PDF?",
                            "Abrir PDF",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (abrir == DialogResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = guardar.FileName,
                                UseShellExecute = true
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "Error al generar PDF: " + ex.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }

        }
        private void ExportarDataGridViewAPDF(string rutaArchivo)
        {
            // Documento horizontal carta
            Document documento = new Document(PageSize.LETTER.Rotate(), 20, 20, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(
                documento,
                new FileStream(rutaArchivo, FileMode.Create)
            );

            documento.Open();

            // ===== TÍTULO =====
            var fuenteTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            Paragraph titulo = new Paragraph("REPORTE DE VENTAS", fuenteTitulo)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10
            };
            documento.Add(titulo);

            // ===== INFORMACIÓN DE FILTROS =====
            var fuenteInfo = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

            string categoriaTexto = "Todas";
            if (cmbCategoria.SelectedIndex >= 0)
                categoriaTexto = cmbCategoria.Text;  // muestra el Nombre de la categoría

            string fechaInicio = dtpInicio.Value.ToString("dd/MM/yyyy");
            string fechaFin = dtpFin.Value.ToString("dd/MM/yyyy");

            Paragraph info = new Paragraph
            {
                Font = fuenteInfo,
                SpacingAfter = 10
            };
            info.Add($"Categoría: {categoriaTexto}\n");
            info.Add($"Período: {fechaInicio} al {fechaFin}\n");
            info.Add($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n");
            documento.Add(info);

            documento.Add(new Paragraph(" ")); // espacio

            // ===== TABLA CON LOS DATOS DEL DGV =====
            PdfPTable tablaPDF = new PdfPTable(dgvReporte.Columns.Count)
            {
                WidthPercentage = 100
            };

            // Anchos iguales para todas las columnas (simple y seguro)
            float[] anchos = new float[dgvReporte.Columns.Count];
            for (int i = 0; i < anchos.Length; i++)
                anchos[i] = 15f;
            tablaPDF.SetWidths(anchos);

            var fuenteEncabezado = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var fuenteCelda = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

            // ---- Encabezados ----
            foreach (DataGridViewColumn columna in dgvReporte.Columns)
            {
                PdfPCell celda = new PdfPCell(new Phrase(columna.HeaderText, fuenteEncabezado))
                {
                    BackgroundColor = new BaseColor(52, 73, 94),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 5
                };
                tablaPDF.AddCell(celda);
            }

            // ---- Filas ----
            bool filaAlterna = false;

            foreach (DataGridViewRow fila in dgvReporte.Rows)
            {
                if (fila.IsNewRow) continue;

                foreach (DataGridViewCell celda in fila.Cells)
                {
                    string valor = celda.Value?.ToString() ?? string.Empty;

                    // Formatear posibles montos de dinero (por nombre de columna)
                    string nombreCol = celda.OwningColumn.Name.ToLower();
                    if (nombreCol.Contains("precio") || nombreCol.Contains("total"))
                    {
                        if (decimal.TryParse(valor, out decimal numero))
                        {
                            valor = numero.ToString("C2"); // moneda local
                        }
                    }

                    PdfPCell celdaPDF = new PdfPCell(new Phrase(valor, fuenteCelda))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 5
                    };

                    if (filaAlterna)
                        celdaPDF.BackgroundColor = new BaseColor(236, 240, 241); // gris claro

                    tablaPDF.AddCell(celdaPDF);
                }

                filaAlterna = !filaAlterna;
            }

            documento.Add(tablaPDF);

            documento.Add(new Paragraph(" ")); // espacio

            // ===== TOTALES (labels del formulario) =====
            var fuenteTotal = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);

            // Total de ventas (lbTotal)
            Paragraph totalParrafo = new Paragraph(lbTotal.Text, fuenteTotal)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingBefore = 10
            };
            documento.Add(totalParrafo);

            // Ganancia bruta (lbGananciaBruta) – solo texto, no está en el DGV
            Paragraph gananciaParrafo = new Paragraph(lbGanacia.Text, fuenteTotal)
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingBefore = 5
            };
            documento.Add(gananciaParrafo);

            documento.Close();
            writer.Close();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cmbCategoria.SelectedIndex = 0;

            dtpInicio.Value = DateTime.Now.AddMonths(-1);
            dtpFin.Value = DateTime.Now;

            dgvReporte.DataSource = null;

            lbTotal.Text = "Total Ventas: $0.00";
            lbGanacia.Text = "Ganancia Bruta: $0.00";
        }
    }
}
