using BowlingScore.FileReaders;
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
        private List<KeyValuePair<string, List<int>>> _nameScoreKvp;

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
                    var fileWrapper = new FileWrapper();

                    IFileReader fileReader = FileReaderSelector.SelectFileReader(extension, fileWrapper);

                    if (fileReader is null)
                    {
                        MessageBoxHelper.ErrorMessageBox("No matching file readers for this file's extension.");
                    }

                    var readFileResult = fileReader.ReadFile(filePath);

                    if (!readFileResult.IsSuccess)
                    {
                        MessageBoxHelper.ErrorMessageBox(readFileResult.Messages[0]);
                    }

                    var bowlingData = fileReader.GetBowlingData((string[]) readFileResult.ResultObject);

                    if (!bowlingData.IsSuccess)
                    {
                        MessageBoxHelper.ErrorMessageBox(bowlingData.Messages[0]);
                    }
                    else
                    {
                        MessageBoxHelper.InfoMessageBox("File processed correctly.");
                        _nameScoreKvp = (List<KeyValuePair<string, List<int>>>) bowlingData.ResultObject;
                    }
                }
            }
        }
    }
}
