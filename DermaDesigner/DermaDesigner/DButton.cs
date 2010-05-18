﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;

namespace DermaDesigner {
	class DButton : Panel {
		private static string respath = "resources/DButton/";

		private static Image topLeftCorner = Derma.LoadImage(respath + "dbutton_upperleft.png");
		private static Image topRightCorner = Derma.LoadImage(respath + "dbutton_upperright.png");
		private static Image bottomLeftCorner = Derma.LoadImage(respath + "dbutton_lowerleft.png");
		private static Image bottomRightCorner = Derma.LoadImage(respath + "dbutton_lowerright.png");

		private static Image topFiller = Derma.LoadImage(respath + "dbutton_topfiller.png");
		private static Image bottomFiller = Derma.LoadImage(respath + "dbutton_bottomfiller.png");
		private static Image leftFiller = Derma.LoadImage(respath + "dbutton_leftfiller.png");
		private static Image rightFiller = Derma.LoadImage(respath + "dbutton_rightfiller.png");
		private static Color wndColor = Color.FromArgb(255, 100, 100, 100);
		private static SolidBrush bgBrush = new SolidBrush(wndColor);

		public static new Image thumbnail = Derma.LoadImage(respath + "dbutton_32.png");

		public static int numOfThisType = 0;
		public new string type = "DButton";
		public new bool canBeChild = true;
		private SizeF labelSize = Derma.GetTextSize("DButton");

		// Lua variables
		string text = "DButton";
		string DoClickFunc = "";

		#region Properties
		[CategoryAttribute("Lua Attributes"), DescriptionAttribute("Sets the text displayed on the DButton")]
		public string Text {
			get { return text; }
			set { 
				this.text = value;
				this.labelSize = Derma.GetTextSize(this.text);
				Derma.Repaint();
			}
		}

		[Editor(typeof(System.ComponentModel.Design.MultilineStringEditor),	typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Lua Attributes"), DescriptionAttribute("The function to be run when the button is clicked")]
		public string DoClick {
			get { return DoClickFunc; }
			set { DoClickFunc = value; }
		}
		#endregion Properties

		public DButton(int xpos, int ypos) : base(xpos, ypos, 70, 25) {
			numOfThisType++;

			if (!this.SetVarName("DButton" + numOfThisType.ToString()))
				while (!this.SetVarName("DButton" + numOfThisType.ToString() + Derma.RandomString(4, false)))
					continue;
		}

		public override void PopulateProperties() {
			Derma.prop.propertyGrid.SelectedObject = this;
		}

		public override void ControlPaint(object sender, PaintEventArgs p) {
			p.Graphics.Clip = new Region(new Rectangle(this.x, this.y, this.width, this.height));

			p.Graphics.FillRectangle(bgBrush, this.x, this.y, this.width, this.height);

			// Draw the top and bottom filler bars
			for (int i = 0; i * 2 < this.width; i++) {
				p.Graphics.DrawImage(topFiller, this.x + (i * 2), this.y, 2, 2);
				p.Graphics.DrawImage(bottomFiller, this.x + (i * 2), this.y + this.height - 2, 2, 2);
			}

			// Draw the left and right filler bars
			for (int i = 0; i * 2 < this.height; i++) {
				p.Graphics.DrawImage(leftFiller, this.x, this.y + (i * 2), 2, 2);
				p.Graphics.DrawImage(rightFiller, this.x + this.width - 2, this.y + (i * 2), 2, 2);
			}

			// top left and right corners
			p.Graphics.DrawImage(topLeftCorner, this.x, this.y, 3, 3);
			p.Graphics.DrawImage(topRightCorner, this.x + this.width - 3, this.y, 3, 3);

			// bottom left and right corners
			p.Graphics.DrawImage(bottomLeftCorner, this.x, this.y + this.height - 3, 3, 3);
			p.Graphics.DrawImage(bottomRightCorner, this.x + this.width - 3, this.y + this.height - 3, 3, 3);

			float xpos = (this.width / 2) - (labelSize.Width / 2);
			float ypos = (this.height / 2) - (labelSize.Height / 2);

			// draw label
			p.Graphics.DrawString(this.text, Derma.DefaultFont, Derma.fontBrush, this.x + xpos, this.y + ypos);
		}

		public override string GenerateLua() {
			StringBuilder code = new StringBuilder("\n");
			code.AppendFormat("local {0} = vgui.Create('DButton')\n", this.varname);

			if (this.parent != null)
				code.AppendFormat("{0}:SetParent({1})", this.varname, parent.varname);

			code.AppendFormat("{0}:SetSize({1}, {2})\n", this.varname, this.width, this.height);

			if (this.ShouldCenter())
				code.AppendFormat("{0}:Center()\n", this.varname);
			else
				code.AppendFormat("{0}:SetPos({1}, {2})\n", this.varname, this.GetPosRelativeToParent().X, this.GetPosRelativeToParent().Y);

			code.AppendFormat("{0}:SizeText('{1}')\n", this.varname, this.text);

			if (!this.visible)
				code.AppendFormat("{0}:SetVisible(false)\n", this.varname);

			if (this.DoClickFunc.Trim() != "")
				code.AppendFormat("{0}.DoClick = function()\n{1}\nend\n", this.varname, this.DoClickFunc);

			return code.ToString();
		}

		public static void Register() {
			Derma.RegisterPanel("DButton", typeof(DButton), thumbnail);
		}
	}
}
