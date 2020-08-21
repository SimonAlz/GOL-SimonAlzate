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
    public partial class ToDialog : Form
    {
        public ToDialog()
        {
            InitializeComponent();
            toNumericUpDown.Minimum = toNumericUpDown.Value;
        }


        // Stop timer when generations hit this number
        public int ToNumber
        {
            get
            {
                return (int)toNumericUpDown.Value;
            }
            set
            {
                toNumericUpDown.Value = value;
            }
        }
    }
}
