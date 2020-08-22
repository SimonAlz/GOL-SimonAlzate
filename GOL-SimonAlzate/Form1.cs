using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace GOL_SimonAlzate
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[30, 30];
        bool[,] scratchPad = new bool[30, 30];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // Font for text
        Font font = new Font("Arial", 12f);

        // The Timer class
        Timer timer = new Timer();

        // Random number generator
        Random rnd = new Random();

        // Generation count
        int generations = 0;

        // Default Boundary Type
        string boundaryType = "Finite";

        // Width and Height of the universe and scratchpad
        int width = 30;
        int height = 30;

        // Limit of generations from the user
        int genLimit = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }
        // Calculate the next generation of cells
        int isAlive = 0;
        private void NextGeneration()
        {
            isAlive = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Read the universe
                    scratchPad[x, y] = false;
                    // count neighbors
                    int count = 0;
                    // If finite is checked
                    if (finiteToolStripMenuItem.Checked == true)
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    // If toroidal is checked
                    else if (toroidalToolStripMenuItem.Checked == true)
                    {
                        count = CountNeighborsToroidal(x, y);
                    }
                    // Apply rules
                    // Any living cell in the current universe with less than 2 living neighbors dies in the next generation
                    if (universe[x,y] == true && count < 2)
                    {
                        scratchPad[x, y] = false;
                    }
                    // Any living cell with more than 3 living neighbors will die in the next generation
                    else if (universe[x,y] == true && count > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    // Any living cell with 2 or 3 living neighbors will live on into the next generation
                    if (universe[x, y] == true && (count == 2 || count == 3))
                    {
                        scratchPad[x, y] = true;
                    }
                    // Any dead cell with exactly 3 living neighbors will be born into the next generation as if by reproduction.
                    if (universe[x, y] == false && count == 3)
                    {
                        scratchPad[x, y] = true;
                    }

                    if (scratchPad[x,y])
                    {
                        isAlive++;
                    }
                }
            }
            // Copy the ScratchPad to the universe
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString() + "\t Alive: " + isAlive.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / (float)universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / (float)universe.GetLength(1);

            // A pen for drawing the grid lines
            Pen gridPen = new Pen(Color.Black, 1);

            // Brush for the HUD
            Brush HUDbrush = new SolidBrush(Color.DarkRed);

            Rectangle rect = graphicsPanel1.ClientRectangle;

            // A Pen for drawing the grid lines (color, width)
            if (gridToolStripMenuItem.Checked == true)
            {
                gridPen = new Pen(gridColor, 1);
            }
            else if (gridToolStripMenuItem.Checked == false)
            {
                // Transparent if the grid has been set to false
                gridPen = new Pen(Color.Transparent, 1);
            }

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = (float)x * (float)cellWidth;
                    cellRect.Y = (float)y * (float)cellHeight;
                    cellRect.Width = (float)cellWidth;
                    cellRect.Height = (float)cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Placement of the HUD in the window
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Far;

            // boundaryType will change depending on which one is selected
            if (finiteToolStripMenuItem.Checked == true)
            {
                boundaryType = "Finite";
            }
            else if (toroidalToolStripMenuItem.Checked == true)
            {
                boundaryType = "Toroidal";
            }

            // Printing the HUD
            if (hudToolStripMenuItem.Checked == true)
            {
                e.Graphics.DrawString("Generations: " + generations + "\nCell count: " + isAlive + "\nBoundary Type: " + boundaryType + "\nUniverse size: {Width = " + width + ", Height = " + height + "}", font, HUDbrush, rect, stringFormat);
            }
            // If HUD is false then make it transparent
            else if (hudToolStripMenuItem.Checked == false)
            {
                e.Graphics.DrawString("Generations: " + generations + "\nCell count: " + isAlive + "\nBoundary Type: " + boundaryType + "\nUniverse size: {Width = " + width + ", Height = " + height + "}" , font, Brushes.Transparent, rect, stringFormat);
            }

            if (generations == genLimit)
            {
                timer.Enabled = false;
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            HUDbrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = (int)(e.X / cellWidth);
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = (int)(e.Y / cellHeight);

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        // Exit program
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // New button
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Iterate through the universe in the y, left to right
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    scratchPad[x, y] = false;
                }
            }
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // Start button
        private void startToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        // Pause button
        private void pauseToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        // Next button
        private void nextToolStripButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    if (xCheck < 0 || xCheck > universe.Length || yCheck < 0 || yCheck > universe.Length)
                    {
                        continue;
                    }
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }
                    if (universe[xCheck, yCheck] == true) count++;


                }
	
            }
            return count;

        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;	
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);	
            for (int yOffset = -1; yOffset <= 1; yOffset++)	
            {	
                for (int xOffset = -1; xOffset <= 1; xOffset++)	
                {	
                    int xCheck = x + xOffset;	
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1	
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0	
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0	
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;	
                }	
            }	
            return count;	
        }
        // Method to Change Back Color
        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        // Method to change cell color
        private void cellColorToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = cellColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        // Method to change grid Color
        private void gridColorToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = gridColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        // Method the grid disappear and appear
        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.Invalidate();
        }

        // Randomize universe from time
        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Create random number for a random universe
                   int num = rnd.Next(0,2);
                    if (num == 0)
                    {
                        universe[x, y] = true;
                    }
                    else if (num == 1 || num == 2)
                    {
                        universe[x, y] = false;
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }

        // Change from toroidal to finite
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }

        // Change from finite to toroidal
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Checked = true;
            finiteToolStripMenuItem.Checked = false;
            graphicsPanel1.Invalidate();
        }

        // Options dialog box
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog();
            dlg.MilisecondsNumber = timer.Interval;
            dlg.WidthNumber = width;
            dlg.HeightNumber = height;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                timer.Interval= dlg.MilisecondsNumber;
                width = dlg.WidthNumber;
                height = dlg.HeightNumber;
                universe = new bool[width, height];
                scratchPad = new bool[width, height];
                graphicsPanel1.Invalidate();
            }
        }

        private void hudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.Invalidate();
        }

        // Reset button
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    scratchPad[x, y] = false;
                }
            }
            // Reset variables to normal
            timer.Interval = 100;
            width = 30;
            height = 30;
            universe = new bool[30, 30];
            scratchPad = new bool[30, 30];
            graphicsPanel1.Invalidate();
        }

        // Option to run to a specific generation
        private void toToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToDialog dlg = new ToDialog();
            dlg.ToNumber = genLimit + 1;
            dlg.Minumum = generations + 1;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                genLimit = dlg.ToNumber;
                timer.Enabled = true;
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                // Iterate through the universe one row at a time.
                for (int y = 0; y < height; y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < width; x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x,y] == true)
                        {
                            currentRow = "O";
                        }

                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else if (universe[x,y] == false)
                        {
                            currentRow = ".";
                        }
                        writer.Write(currentRow);
                    }
                    writer.Write("\n");
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row == "!")
                    {
                        continue;
                    }

                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    if (row != "!")
                    {
                        maxHeight++;
                    }

                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row == "!")
                    {
                        continue;
                    }
                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    for (int xPos = 0; xPos < row.Length; xPos++)
                    {
                        // If row[xPos] is a 'O' (capital O) then
                        // set the corresponding cell in the universe to alive.
                        if (row[xPos] == 'O')
                        {
                            universe[xPos, xPos] = true;
                        }

                        // If row[xPos] is a '.' (period) then
                        // set the corresponding cell in the universe to dead.
                        if (row[xPos] == '.')
                        {
                            universe[xPos, xPos] = false;
                        }
                    }
                }

                // Close the file.
                reader.Close();
            }
            graphicsPanel1.Invalidate();
        }
    }
}
