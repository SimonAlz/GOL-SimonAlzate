using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL_SimonAlzate
{
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
        }

        // Changing number of Milisecods in a generation
        public int MilisecondsNumber
        {
            get
            {
                return (int)milisecondsNumericUpDown.Value;
            }
            set
            {
                milisecondsNumericUpDown.Value = value;
            }
        }

        // Changing Width of the universe
        public int WidthNumber
        {
            get
            {
                return (int)widthNumericUpDown.Value;
            }
            set
            {
                widthNumericUpDown.Value = value;
            }
        }

        // Changing Height of the universe
        public int HeightNumber
        {
            get
            {
                return (int)heightNumericUpDown.Value;
            }
            set
            {
                heightNumericUpDown.Value = value;
            }
        }
    }
}
