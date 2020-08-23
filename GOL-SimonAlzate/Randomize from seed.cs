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
    public partial class Randomize_from_seed : Form
    {
        public Randomize_from_seed()
        {
            InitializeComponent();
        }

        // Set the seed from the user into a random universe
        public int randomSeed
        {
            get
            {
                return (int)seedNumericUpDown.Value;
            }
            set
            {
                seedNumericUpDown.Value = value;
            }
        }
    }
}
