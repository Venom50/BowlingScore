using BowlingScore.Calculator;
using BowlingScore.Controller;
using BowlingScore.FileReaders;
using BowlingScore.Helpers;
using BowlingScore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace BowlingScore
{
    public partial class Form1 : Form
    {
        private List<BowlingScoreModel> _bowlingScoreModels;

        public Form1()
        {
            InitializeComponent();
            _bowlingScoreModels = new List<BowlingScoreModel>();
        }

        private void uploadFileButton_Click(object sender, EventArgs e)
        {
            UseOpenFileDialog("c:\\", new[] { OpenFileDialogFilters.TXT_FILTER });
            ListAllBowlingScoreModelsInListView();
        }

        private void UseOpenFileDialog(string initialCatalog, string[] fileDialogFilters)
        {
            var bowlingScoreCalculator = new BowlingScoreCalculator();
            var bowlingDataController = new BowlingDataController(bowlingScoreCalculator);

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = initialCatalog;
                openFileDialog.Filter = fileDialogFilters.Length > 1 ? string.Join("|", fileDialogFilters) : fileDialogFilters[0];
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    var fileReader = FileReaderSelector.SelectFileReader(Path.GetExtension(filePath).ToLower(), new FileWrapper());
                    var result = bowlingDataController.GetBowlingScoreModelsFromFile(filePath, fileReader);

                    if (!result.IsSuccess)
                    {
                        MessageBoxHelper.ErrorMessageBox(result.Messages[0]);
                        return;
                    }

                    MessageBoxHelper.InfoMessageBox("File processed correctly.");

                    listView1.Items.Clear();

                    var nameScoreKvp = (List<KeyValuePair<string, List<int>>>)result.ResultObject;
                    _bowlingScoreModels = bowlingDataController.AddBowlingScoreModelsToList(nameScoreKvp);
                }
            }
        }

        private void ListAllBowlingScoreModelsInListView()
        {
            foreach (var bowlingScoreModel in _bowlingScoreModels)
            {
                ListViewItem item = new ListViewItem(bowlingScoreModel.Name);
                item.SubItems.Add(bowlingScoreModel.TotalScore.ToString());

                foreach (var score in bowlingScoreModel.ThrowsScores)
                {
                    item.SubItems.Add(score.ToString());
                }

                listView1.Items.Add(item);
            }

            _bowlingScoreModels.Clear();
        }
    }
}
