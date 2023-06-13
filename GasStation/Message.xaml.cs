using System.Windows;

namespace GasStation
{
    /// <summary>
    /// Реакция приложения на действия пользователя
    /// </summary>
    public partial class Message : Window
    {
        private Result _rez;
        public Result Rez => _rez;

        public enum Type
        {
            OK, OKCancel, YesNo, YesNoCancel
        }

        public enum Result
        {
            OK , Cancel, Yes, No
        }

        public Message(string caption, string info, Type mt)
        {
            InitializeComponent();
            
            this.caption.Content = caption;
            this.info.Text = info;

            switch (mt)
            {
                case Type.OK:
                    {
                        buttons.Items.Remove(CancelLvi);
                        buttons.Items.Remove(YesLvi);
                        buttons.Items.Remove(NoLvi);

                        break;
                    }
                case Type.OKCancel:
                    {
                        buttons.Items.Remove(YesLvi);
                        buttons.Items.Remove(NoLvi);

                        break;
                    }
                case Type.YesNo:
                    {
                        buttons.Items.Remove(OKLvi);
                        buttons.Items.Remove(CancelLvi);

                        break;
                    }
                case Type.YesNoCancel:
                    {
                        buttons.Items.Remove(OK);

                        break;
                    }
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e) => _rez = Result.OK;

        private void Cancel_Click(object sender, RoutedEventArgs e) => _rez = Result.Cancel;

        private void Yes_Click(object sender, RoutedEventArgs e) => _rez = Result.Yes;

        private void No_Click(object sender, RoutedEventArgs e) => _rez = Result.No;
    }
}