using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBwin
	{
	internal class ComboBoxMod : ComboBox
		{

		public bool _enabled;

		public new bool Enabled
			{
			get { return _enabled; }
			set { _enabled = value; }
			}

		int _previousSelIndex;

		public ComboBoxMod() : base()
			{
			this.Enabled = true;
			}

		//protected override void OnPaint(PaintEventArgs e)
		//	{
		//	base.OnPaint(e);
		//	e.Graphics.FillRectangle(Brushes.Yellow,this.ClientRectangle);
		//	e.Graphics.DrawString(this.Text, this.Font, Brushes.Blue, this.Location);
		//	}


		protected override void OnSelectedIndexChanged(EventArgs e)
			{	

			if(Enabled)
				{
				base.OnSelectedIndexChanged(e);
				_previousSelIndex = this.SelectedIndex;
				}
			else
				{
				this.SelectedIndex = _previousSelIndex;	// Annulla la modifica. se disabilitato
				}
			}

		protected override void OnTextChanged(EventArgs e)
			{
			if(Enabled)
				{
				base.OnTextChanged(e);
				_previousSelIndex = this.SelectedIndex;
				}
			else
				{
				this.SelectedIndex = _previousSelIndex;	// Annulla la modifica. se disabilitato
				}
			}
		}
	}
