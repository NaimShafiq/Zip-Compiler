using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zip_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool isFolder = false;
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string zipFileName = Path.ChangeExtension(saveFileDialog1.FileName, "zip");

                using (ZipArchive zip = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
                {
                    foreach (string folder in listBox1.Items)
                    {
                        string directoryToSkip = "C:\\MyFolder\\SubfolderToSkip"; // Specify the directory to skip here
                        ZipDirectory(zip, folder, Path.GetFileName(folder), directoryToSkip);
                    }
                }

                MessageBox.Show("ZIP file created successfully!");
            }
        }


        private void ZipDirectory(ZipArchive zip, string sourceFolder, string entryPrefix, string directoryToSkip)
        {
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string entryName = Path.Combine(entryPrefix, Path.GetFileName(file));
                zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
            }

            string[] subFolders = Directory.GetDirectories(sourceFolder);
            foreach (string subFolder in subFolders)
            {
                if (subFolder.Equals(directoryToSkip, StringComparison.OrdinalIgnoreCase))
                {
                    continue; // Skip the directory
                }

                string entryName = Path.Combine(entryPrefix, Path.GetFileName(subFolder));
                ZipDirectory(zip, subFolder, entryName, directoryToSkip);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog1.SelectedPath;
                listBox1.Items.Add(selectedFolder);
                isFolder = true;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "All Files (*.*)|*.*"; // Set an appropriate file filter
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] files = openFileDialog1.FileNames;
                listBox1.Items.Clear(); // Clear existing items in the ListBox
                listBox1.Items.AddRange(files); // Add the selected file paths to the ListBox
                isFolder = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

    }
}