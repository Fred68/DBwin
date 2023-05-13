using System;
using System.Drawing;
using System.Windows.Forms;

namespace DBwin
	{
	public partial class InputForm : Form
		{
		TextBox[] tb;
		Label[] lb;
		string[] res;
		string help = string.Empty;
		/// <summary>
		/// Prepara form per login o altro
		/// </summary>
		/// <param name="label">Nomi dei campi (@ per tutto minuscolo)</param>
		/// <param name="result"></param>
		public InputForm(string title, string[] label, out string[] result, string titleOk = "", string titleCancel = "", string helpMsg = "")
			{
			InitializeComponent();

			this.Text = title;
			tb1.Enabled = tb1.Visible = lb1.Enabled = lb1.Visible = true;
			int passo = (int)(tb1.Height * 1.2);
			int passoBtn = (int)(tb1.Height * 1.4);
			tb = new TextBox[label.Length];
			lb = new Label[label.Length];
			res = new string[label.Length];

			btAnnulla.Location = new Point(btOK.Location.X, btOK.Location.Y + passoBtn);

			help = helpMsg;
			int hlp = 0;
			if (help == string.Empty)
				{
				btHelp.Visible = false;
				hlp = 0;
				}
			else
				{
				btHelp.Visible = true;
				btHelp.Size = btOK.Size;
				btHelp.Location = new Point(btAnnulla.Location.X, btAnnulla.Location.Y + passoBtn);
				hlp = 1;
				}

			this.Height += passo * (label.Length - 2 + hlp);


			for (int i = 0; i < label.Length; i++)
				{
				tb[i] = new TextBox();
				tb[i].Name = "TB" + "0";
				tb[i].Size = tb1.Size;
				tb[i].Left = tb1.Left;
				tb[i].Top = tb1.Top + passo * i;
				if (label[i].Substring(0, 1) == "@")
					{
					tb[i].CharacterCasing = CharacterCasing.Lower;
					label[i] = label[i].Substring(1);
					}

				lb[i] = new Label();
				lb[i].Name = "LB" + "0";
				lb[i].Size = lb1.Size;
				lb[i].Left = lb1.Left;
				lb[i].Top = lb1.Top + passo * i;
				lb[i].Text = label[i];

				this.Controls.Add(tb[i]);
				this.Controls.Add(lb[i]);
				tb[i].Visible = lb[i].Visible = true;
				tb[i].Enabled = lb[i].Enabled = true;
				}
			tb1.Enabled = tb1.Visible = lb1.Enabled = lb1.Visible = false;

			if (titleOk.Length > 0) btOK.Text = titleOk;
			if (titleCancel.Length > 0) btAnnulla.Text = titleCancel;

			result = res;

			Invalidate();

			}

		private void btOK_Click(object sender, EventArgs e)
			{
			for (int i = 0; i < res.Length; i++)
				{
				res[i] = tb[i].Text;
				}
			DialogResult = DialogResult.OK;
			Close();
			}

		private void Annulla_Click(object sender, EventArgs e)
			{
			DialogResult = DialogResult.Cancel;
			Close();
			}

		private void btHelp_Click(object sender, EventArgs e)
			{
			if (help != string.Empty)
				{
				MessageBox.Show(help);
				}

			}
		}
	}
