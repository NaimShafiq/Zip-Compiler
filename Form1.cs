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
                    bool errorOccurred = false; // Track if an error occurs
                    bool zipCreated = false; // Track if ZIP file creation was successful

                    try
                    {
                        using (ZipArchive zip = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
                        {
                            string directoryToSkip = "C:\\MyFolder\\SubfolderToSkip"; // Specify the directory to skip here
                            HashSet<string> addedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                            foreach (string item in listBox1.Items)
                            {
                                if (File.Exists(item))
                                {
                                    string entryPrefix = Path.GetFileName(item);
                                    try
                                    {
                                        ZipDirectory(zip, item, entryPrefix, directoryToSkip, addedFiles);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                        errorOccurred = true;
                                        break; // Exit the loop when an error occurs
                                    }
                                }
                                else if (Directory.Exists(item))
                                {
                                    string entryPrefix = Path.GetFileName(item);
                                    try
                                    {
                                        ZipDirectory(zip, item, entryPrefix, directoryToSkip, addedFiles);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                        errorOccurred = true;
                                        break; // Exit the loop when an error occurs
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Source path does not exist: " + item);
                                    errorOccurred = true;
                                    break; // Exit the loop when an error occurs
                                }
                            }

                            // Set the flag indicating that the ZIP file was successfully created
                            zipCreated = true;
                        }

                        if (!errorOccurred && zipCreated) // Display success message only if no error occurred and ZIP file was created
                        {
                            MessageBox.Show("ZIP file created successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        errorOccurred = true;
                    }

                    // Delete the ZIP file if it was created but an error occurred
                    if (errorOccurred && zipCreated)
                    {
                        File.Delete(zipFileName);
                    }
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

        private void ZipDirectory(ZipArchive zip, string sourcePath, string entryPrefix, string directoryToSkip, HashSet<string> addedFiles)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    // If the source path is a file, add it to the ZIP archive
                    string entryName = entryPrefix;

                    if (addedFiles.Contains(entryName))
                    {
                        throw new Exception($"File with the same name already exists in the ZIP archive: {entryName}");
                    }

                    zip.CreateEntryFromFile(sourcePath, entryName, CompressionLevel.Optimal);
                    addedFiles.Add(entryName);
                }
                else if (Directory.Exists(sourcePath))
                {
                    // If the source path is a directory, add its contents to the ZIP archive
                    string[] files = Directory.GetFiles(sourcePath);
                    foreach (string file in files)
                    {
                        if (Path.GetExtension(file).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            // If the file is a text file, add it directly to the ZIP archive
                            string entryName = Path.GetFileName(file);

                            if (addedFiles.Contains(entryName))
                            {
                                throw new Exception($"File with the same name already exists in the ZIP archive: {entryName}");
                            }

                            zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                            addedFiles.Add(entryName);
                        }
                        else
                        {
                            // If the file is not a text file, recursively add it to the ZIP archive
                            string entryName = Path.Combine(entryPrefix, Path.GetFileName(file));
                            ZipDirectory(zip, file, entryName, directoryToSkip, addedFiles);
                        }
                    }

                    string[] subFolders = Directory.GetDirectories(sourcePath);
                    foreach (string subFolder in subFolders)
                    {
                        if (subFolder != directoryToSkip && !IsSubfolderOf(subFolder, directoryToSkip))
                        {
                            string entryName = Path.Combine(entryPrefix, Path.GetFileName(subFolder));
                            ZipDirectory(zip, subFolder, entryName, directoryToSkip, addedFiles);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Source path does not exist: " + sourcePath);
                    return; // Terminate the process here
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return; // Terminate the process here
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

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                ZipArchive zip = ZipFile.OpenRead(openFileDialog1.FileName);
                listBox2.Items.Clear();
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    listBox2.Items.Add(entry.FullName);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
                DialogResult result2 = folderBrowserDialog1.ShowDialog();
                if (result2 == DialogResult.OK)
                {
                    string destinationFolder = folderBrowserDialog1.SelectedPath;
                    using (ZipArchive archive = ZipFile.OpenRead(openFileDialog1.FileName))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string destinationPath = Path.Combine(destinationFolder, entry.FullName);
                            if (File.Exists(destinationPath))
                            {
                                MessageBox.Show("The file already exists: " + destinationPath);
                            }
                            else
                            {
                                entry.ExtractToFile(destinationPath);
                            }
                        }
                    }
                    MessageBox.Show("ZIP file extracted successfully!");
                }
            }
        }
    }
}