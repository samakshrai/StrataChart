using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using StrataChart;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

public partial class StrataVisual : System.Web.UI.Page
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StrataDB"].ToString());
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var layers = GetStrataLayers();

            double currentTop = 0;
            foreach (var layer in layers)
            {
                double thickness = (layer.EndDepth - layer.StartDepth) * 8; // visual scale: 7px per meter

                // Add colored strata block
                Panel panel = new Panel
                {
                    CssClass = "strata-block " + layer.PatternCssClass,
                    Height = new Unit(thickness, UnitType.Pixel),
                    ToolTip = $"{layer.Material}: {layer.StartDepth}–{layer.EndDepth} m"
                };
                panel.Style["position"] = "absolute";
                panel.Style["top"] = $"{currentTop}px";
                panel.Style["left"] = "100px"; // Adjust as needed
                strataPanel.Controls.Add(panel); // ✅ Add the panel to the UI

                // Add material label (to the left side)
                Literal label = new Literal();
                label.Text = $@"
                <div class='label-text' style='top:{currentTop}px; height:{thickness}px; line-height:{thickness}px'>
                 {layer.Material}
                </div>";
                strataPanel.Controls.Add(label);

                // Add depth label (to the right side)
                Literal depthLabel = new Literal();
                double labelTop = currentTop + thickness - 14; // Align bottom of block
                depthLabel.Text = $"<div class='depth-label' style='top:{labelTop}px'>{layer.EndDepth:0.00} m</div>";
                strataPanel.Controls.Add(depthLabel);

                currentTop += thickness;
            }
        }
    }

    private List<StrataLayer> GetStrataLayers()
    {
        List<StrataLayer> layers = new List<StrataLayer>();

        {
            conn.Open();
            string query = "GetStrataLayers";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    StrataLayer layer = new StrataLayer
                    {
                        Material = reader["Material"].ToString(),
                        StartDepth = Convert.ToDouble(reader["StartDepth"]),
                        EndDepth = Convert.ToDouble(reader["EndDepth"]),
                        PatternCssClass = reader["PatternCssClass"].ToString()
                    };
                    layers.Add(layer);
                }
            }
        }

        return layers;
    }

}
