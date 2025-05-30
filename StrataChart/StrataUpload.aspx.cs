using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
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
                    LoadStrataDiagram(); // This must pull data from SQL
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

        private void LoadStrataDiagram()
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["StrataDB"].ConnectionString;

            List<StrataChart.StrataLayer> layers = new List<StrataChart.StrataLayer>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Material, StartDepth, EndDepth, PatternCssClass FROM StrataLayers ORDER BY StartDepth", conn);
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

                
                Literal label = new Literal();
                label.Text = $"<div class='label-text' style='top:{currentTop}px'>{layer.Material}</div>";
                strataPanel.Controls.Add(label);

                
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
        }

        private void SaveExcelToDatabase()
        {
            if (FileUpload1.HasFile)
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var stream = FileUpload1.PostedFile.InputStream)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    DataTable table = result.Tables[0]; // assuming first sheet

                    try
                    {
                        string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["StrataDB"].ConnectionString;

                        using (SqlConnection conn = new SqlConnection(connStr))
                        {
                            conn.Open();

                            foreach (DataRow row in table.Rows)
                            {
                                if (row[0].ToString().Trim().ToLower() == "material") continue; // skip header

                                string material = row[0].ToString();
                                double startDepth = Convert.ToDouble(row[1]);
                                double endDepth = Convert.ToDouble(row[2]);
                                string patternCss = row[3].ToString();

                                SqlCommand cmd = new SqlCommand("INSERT INTO StrataLayers (Material, StartDepth, EndDepth, PatternCssClass) VALUES (@Material, @Start, @End, @Css)", conn);
                                cmd.Parameters.AddWithValue("@Material", material);
                                cmd.Parameters.AddWithValue("@Start", startDepth);
                                cmd.Parameters.AddWithValue("@End", endDepth);
                                cmd.Parameters.AddWithValue("@Css", patternCss);

                                cmd.ExecuteNonQuery();
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

    }

}
