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
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "ZIP files (*.zip)|*.zip";
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string zipFileName = saveFileDialog.FileName;

                    using (ZipArchive zip = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
                    {
                        string directoryToSkip = "C:\\MyFolder\\SubfolderToSkip"; // Specify the directory to skip here
                        foreach (string folder in listBox1.Items)
                        {
                            if (folder != directoryToSkip && !IsSubfolderOf(folder, directoryToSkip))
                            {
                                ZipDirectory(zip, folder, Path.GetFileName(folder), directoryToSkip);
                            }
                        }
                    }

                    MessageBox.Show("ZIP file created successfully!");
                }
            }
        }

        private bool IsSubfolderOf(string folderPath, string parentFolderPath)
        {
            DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
            DirectoryInfo parentFolderInfo = new DirectoryInfo(parentFolderPath);

            while (folderInfo.Parent != null)
            {
                if (folderInfo.Parent.FullName == parentFolderInfo.FullName)
                {
                    return true;
                }

                folderInfo = folderInfo.Parent;
            }

            return false;
        }


        private void ZipDirectory(ZipArchive zip, string sourcePath, string entryPrefix, string directoryToSkip)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    // If the source path is a file, add it to the ZIP archive
                    string entryName = Path.Combine(entryPrefix, Path.GetFileName(sourcePath));
                    zip.CreateEntryFromFile(sourcePath, entryName, CompressionLevel.Optimal);
                }
                else if (Directory.Exists(sourcePath))
                {
                    // If the source path is a directory, add its contents to the ZIP archive
                    string[] files = Directory.GetFiles(sourcePath);
                    foreach (string file in files)
                    {
                        string entryName = Path.Combine(entryPrefix, Path.GetFileName(file));
                        zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                    }

                    string[] subFolders = Directory.GetDirectories(sourcePath);
                    foreach (string subFolder in subFolders)
                    {
                        if (subFolder != directoryToSkip && !IsSubfolderOf(subFolder, directoryToSkip))
                        {
                            string entryName = Path.Combine(entryPrefix, Path.GetFileName(subFolder));
                            ZipDirectory(zip, subFolder, entryName, directoryToSkip);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Source path does not exist: " + sourcePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while zipping the directory: " + ex.Message);
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