using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace RateCalculator
{
    public partial class Form1 : Form
    {
        private string inputFilePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV Files|*.csv";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    inputFilePath = ofd.FileName;
                    label1.Text = "File selected: " + inputFilePath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show("Please select a file first.");
                return;
            }

            try
            {
                List<string[]> rows = ReadCsvFile(inputFilePath);

                if (rows.Count < 2)
                {
                    MessageBox.Show("No data found in the CSV file.");
                    return;
                }

                // Validate the CSV columns.
                if (rows[0].Length < 4)
                {
                    MessageBox.Show("The CSV file does not have the expected number of columns.");
                    return;
                }

                // Inserting "loan_rate" header to CSV
                rows[0] = rows[0].Concat(new[] { "loan_rate" }).ToArray();

                for (int i = 1; i < rows.Count; i++)
                {
                    double pv = SafeConvertToDouble(rows[i][2]);
                    double pmt = -SafeConvertToDouble(rows[i][3]);
                    double nper = SafeConvertToDouble(rows[i][4]);

                    var mortgage = new Mortgage(pv, pmt, nper);

                    var rateValue = double.IsNaN(mortgage.Rate) ? "Not converged" : (mortgage.Rate * 12 * 100).ToString("F2");

                    rows[i] = rows[i].Concat(new[] { rateValue }).ToArray();
                }

                string outputPath = inputFilePath.Replace(".csv", "_output.csv");
                WriteCsvFile(outputPath, rows);

                label1.Text = "Processing completed. \r\nOutput saved as " + outputPath;
            }
            catch (Exception ex)
            {
                label1.Text = "Error occurred: " + ex.Message;
            }
        }

        private List<string[]> ReadCsvFile(string filePath)
        {
            var result = new List<string[]>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    result.Add(values);
                }
            }
            return result;
        }

        private void WriteCsvFile(string filePath, List<string[]> data)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var row in data)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }
        }

        private double SafeConvertToDouble(string input)
        {
            if (double.TryParse(input, out double result))
            {
                return result;
            }
            else
            {
                throw new FormatException($"Could not convert '{input}' to a double.");
            }
        }
    }
}
