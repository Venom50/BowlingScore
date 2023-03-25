using BowlingScore.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BowlingScore
{
    public partial class Form1 : Form
    {
        private List<KeyValuePair<string, List<string>>> _nameScoreKvp;

        public Form1()
        {
            InitializeComponent();
        }

        private void uploadFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = OpenFileDialogFilters.TXT_FILTER;
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    var extension = Path.GetExtension(filePath).ToLower();

                    IFileReader fileReader = FileReaderSelector.SelectFileReader(extension);

                    var readFileResult = fileReader.ReadFile(filePath);

                    if (!readFileResult.IsSuccess)
                    {
                        if (readFileResult.Messages.Count > 1)
                        {
                            MessageBoxHelper.ErrorMessageBox(string.Join(Environment.NewLine, readFileResult.Messages));
                        }
                        else
                        {
                            MessageBoxHelper.ErrorMessageBox(readFileResult.Messages[0]);
                        }
                    }
                    else
                    {
                        MessageBoxHelper.InfoMessageBox("File processed correctly.");
                        _nameScoreKvp = (List<KeyValuePair<string, List<string>>>) readFileResult.ResultObject;
                    }
                }
            }
        }
    }
}
