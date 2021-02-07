using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

/**
 * Name: Aaron Audet
 * Student Number: 000367464
 * File Date: November 22, 2020
 * Program purpose: The porpuse of the application is to check to see if an html file has balanced tags.
 * Statement of Authorship: I, Aaron Audet, 000367464 certify that this material is my original work.  No other person's work has been used without due acknowledgement.
 */

namespace Lab4B
{
    public partial class Form1 : Form
    {
        Stack<string> fileContent = new Stack<string>();
        Stack<string> fileName = new Stack<string>();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Loads the selected file into the application
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Clear App
            checkTags.Enabled = false;
            fileStatus.Text = "No File Loaded";
            fileOutput.Items.Clear();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // File picker
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "HTML files(*.html)|*.html";

                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get file name
                    fileName.Push(Path.GetFileName(openFileDialog.FileName));

                    //Input file content
                    var fileStream = openFileDialog.OpenFile();
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        Regex tags = new Regex(@"<.+?>");
                        var text = reader.ReadToEnd();
                        MatchCollection matches = tags.Matches(text);
                        int i = 0;
                        Stack<string> reverseTags = new Stack<string>();

                        // Remove attributes
                        foreach(Match matche in matches)
                        {
                            if(matche.Value.Contains(" "))
                            {
                                string[] split = matche.Value.Split(' ');
                                split[0] += ">";
                                reverseTags.Push(split[0].ToLower());
                            } else
                            {
                                reverseTags.Push(matche.Value.ToLower());
                            }
                        }

                        // Fixing order
                        foreach(var item in reverseTags)
                        {
                            fileContent.Push(item);
                        }
                    }

                    // Updating App
                    checkTags.Enabled = true;
                    fileStatus.Text = $"Loaded: {fileName.First()}";
                    
                }
            }
        }

        /// <summary>
        /// Checks to see if all tags are balanced
        /// </summary>
        /// <param name="sender">Not Used</param>
        /// <param name="e">Not Used</param>
        private void checkTagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int total = 0; // Total amount of items added to date
            int tab = 0; // Indentation amount
            List<string> list = new List<string>();
            bool balance = true;


            // Check Tags
            foreach(var item in fileContent)
            {
                if(total == 0) // If First
                {
                    if(!item.Contains("/"))
                    {
                        fileOutput.Items.Add($"Found opening tag: {item}");
                        list.Add(item);
                        tab++;
                    } else // Not balanced
                    {
                        fileOutput.Items.Add($"Found closing tag: {item}");
                        balance = false;
                        break;
                    }
                } else
                {
                    char t = Convert.ToChar(9);
                    if (!item.Contains("/")) // If Opening
                    {
                        if(item == "<br>" || item == "<hr>" || item == "<img>")
                        {
                            fileOutput.Items.Add($"{new String(t, tab)}Found non-container tag: {item}");
                        } else
                        {
                            fileOutput.Items.Add($"{new String(t, tab)}Found opening tag: {item}");
                            list.Add(item);
                            tab++;
                        }
                    } else if(item.Contains("/")) // If closing
                    {
                        if(list.Last() == item.Remove(1, 1))
                        {
                            tab--;
                            fileOutput.Items.Add($"{new String(t, tab)}Found closing tag: {item}");
                            list.RemoveAt(list.Count - 1);
                        } else // Not balanced
                        {
                            balance = false;
                            fileOutput.Items.Add($"{new String(t, tab)}Found closing tag: {item}");
                            break;
                        }
                    }
                }
                total++;
            }

            // Check balance
            if(balance)
            {
                fileStatus.Text = $"{fileName.First()} has balanced tags";
            } else
            {
                fileStatus.Text = $"{fileName.First()} does not have balanced tags";
            }
        }
    }
}
