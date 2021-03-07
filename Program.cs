using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
namespace PlayString
{
    class Progra
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1 && new List<string>(new string[] { "-?", "/?", "-help", "/help" }).Contains(args[0].ToLowerInvariant()))
            {
                MessageBox.Show(
                    "STR2WAVE - Convert BASIC play command string into an 8 bit 44100hz mono WAV file.\n\n"+
                    "Usage: STR2WAV [string] [file]\n\n"+
                    "Example:\n"+
                    "STR2WAV T240L8CDEFGAB test.wav"
                    );
                return;
            }

            if (args.Length == 2)
            {
                STR2WAV.GenerateWAVFile(args[0], args[1]);
                return;
            }



            // if run with no args, do a simple popup
            string snd = string.Empty;
            if (InputBox("STR2WAV", "Enter PLAY string", ref snd) != DialogResult.OK)
                return;

            // get *.wav save location
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "WAV Files (*.wav)|*.wav";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            STR2WAV.GenerateWAVFile(snd, sfd.FileName);
        }





        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}
