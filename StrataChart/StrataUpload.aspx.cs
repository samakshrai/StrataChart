using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using ExcelDataReader;
using System.Configuration;

namespace StrataChart
{
    public partial class StrataUpload : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StrataDB"].ToString());

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UploadSuccess"] != null && (bool)Session["UploadSuccess"])
                {
                    LoadStrataDiagram();
                    Session.Remove("UploadSuccess");
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            SaveExcelToDatabase();
            Session["UploadSuccess"] = true;
            Response.Redirect(Request.RawUrl);
        }

        private void SaveExcelToDatabase()
        {
            if (FileUpload1.HasFile)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = FileUpload1.PostedFile.InputStream)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    });

                    DataTable table = result.Tables[0];

                    try
                    {
                        // ✅ Generate unique UploadCode
                        Guid uploadCode = Guid.NewGuid();
                        Session["CurrentUploadCode"] = uploadCode;
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StrataDB"].ToString()))
                        {
                            conn.Open();

                            foreach (DataRow row in table.Rows)
                            {
                                string material = row[0].ToString();
                                double startDepth = Convert.ToDouble(row[1]);
                                double endDepth = Convert.ToDouble(row[2]);
                                string pattern = row[3].ToString();

                                // ✅ Insert into DB with UploadCode
                                string query = @"INSERT INTO StrataLayers (Material, StartDepth, EndDepth, PatternCssClass, UploadCode) 
                                                 VALUES (@mat, @start, @end, @pattern, @code)";
                                SqlCommand insertCmd = new SqlCommand(query, conn);
                                insertCmd.Parameters.AddWithValue("@mat", material);
                                insertCmd.Parameters.AddWithValue("@start", startDepth);
                                insertCmd.Parameters.AddWithValue("@end", endDepth);
                                insertCmd.Parameters.AddWithValue("@pattern", pattern);
                                insertCmd.Parameters.AddWithValue("@code", uploadCode);
                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        lblMessage.ForeColor = System.Drawing.Color.Green;
                        lblMessage.Text = "Excel data uploaded and saved.";
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Error: " + ex.Message;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
            else
            {
                lblMessage.Text = "Please upload an Excel (.xlsx) file.";
            }
        }

        private void LoadStrataDiagram()
        {
            if (Session["CurrentUploadCode"] == null) return;
            Guid code = (Guid)Session["CurrentUploadCode"];
            List<StrataChart.StrataLayer> layers = new List<StrataChart.StrataLayer>();
            string connStr = ConfigurationManager.ConnectionStrings["StrataDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Material, StartDepth, EndDepth, PatternCssClass FROM StrataLayers WHERE UploadCode = @code ORDER BY StartDepth", conn);
                cmd.Parameters.AddWithValue("@code", code);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    layers.Add(new StrataChart.StrataLayer
                    {
                        Material = reader["Material"].ToString(),
                        StartDepth = Convert.ToDouble(reader["StartDepth"]),
                        EndDepth = Convert.ToDouble(reader["EndDepth"]),
                        PatternCssClass = reader["PatternCssClass"].ToString()
                    });
                }
            }

            double currentTop = 0;
            foreach (var layer in layers)
            {
                double thickness = (layer.EndDepth - layer.StartDepth) * 7;

                string blockHtml = $@"
                <div class='strata-block {layer.PatternCssClass}' style='top:{currentTop}px; height:{thickness}px;' title='{layer.Material}: {layer.StartDepth}–{layer.EndDepth} m'>
                    <div class='block-label'>{layer.Material}</div>
                </div>";
                strataPanel.Controls.Add(new LiteralControl(blockHtml));

                Literal depthLabel = new Literal();
                depthLabel.Text = $"<div class='depth-label' style='top:{currentTop + thickness}px'>{layer.EndDepth:0.00} m</div>";
                strataPanel.Controls.Add(depthLabel);

                Panel panel = new Panel
                {
                    CssClass = "strata-block " + layer.PatternCssClass,
                    Height = new Unit(thickness, UnitType.Pixel),
                    ToolTip = $"{layer.Material}: {layer.StartDepth}–{layer.EndDepth} m"
                };
                panel.Style["top"] = $"{currentTop}px";
                strataPanel.Controls.Add(panel);

                currentTop += thickness;

            }
            double totalHeight = currentTop;
            strataPanel.Height = new Unit(totalHeight, UnitType.Pixel);
        }
    }
}
