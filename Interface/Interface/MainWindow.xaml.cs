using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Interface
{
    public partial class MassUserControl : UserControl
    {
        public MassUserControl()
        {
            InitializeComponent();

            var viewmodel = new ViewModel();
            this.DataContext = viewmodel;
            
            for (int i = 0; i < viewmodel.MassHeight; i++) ;
                massGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < viewmodel.MassWidth; i++) ;
                massGrid.Definitions.Add(new RowDefinition());


        }
    }
}
