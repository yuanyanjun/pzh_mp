using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();

            lblColor.BackColor = bgColor;
        }

        private bool[,] struArr = new bool[5, 5];
        private Color bgColor = Color.Red;

        private void lblMode_Paint(object sender, PaintEventArgs e)
        {
            Graphics gp = e.Graphics;

            gp.Clear(Color.Black);
            Pen p = new Pen(Color.White);

            for (int i = 31; i < 156; i = i + 31)
            {
                gp.DrawLine(p, 1, i, 155, i);
            }


            for (int i = 31; i < 156; i = i + 31)
            {
                gp.DrawLine(p, i, 1, i, 155);
            }

            var s = new SolidBrush(bgColor);
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    if (struArr[x, y])
                    {
                        gp.FillRectangle(s, 31 * x + 1, 31 * y + 1, 30, 30);
                    }
                }
            }
        }

        private void lblMode_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            int xPos, yPos;
            xPos = e.X / 31;
            yPos = e.Y / 31;

            struArr[xPos, yPos] = !struArr[xPos, yPos];
            bool b = struArr[xPos, yPos];

            using (var gp = lblMode.CreateGraphics())
            {
                SolidBrush s = new SolidBrush(b ? bgColor : Color.Black);
                gp.FillRectangle(s, 31 * xPos + 1, 31 * yPos + 1, 30, 30);
            }
        }

        private void lblColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            bgColor = colorDialog1.Color;

            lblColor.BackColor = bgColor;
            lblMode.Invalidate();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var isEmpty = false;
            foreach (var item in struArr)
            {
                if (item)
                {
                    isEmpty = true;
                    break;
                }
            }

            if (!isEmpty)
            {
                MessageBox.Show("图案为空，请先用鼠标点击坐标区域绘制图案", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var buf = new StringBuilder(512);
            foreach (var item in struArr)
            {
                buf.Append(item ? "1" : "0");
            }

            var blockCode = buf.ToString();
            foreach (ListViewItem item in lsvBlockSet.Items)
            {
                if (item.SubItems[0].Text == blockCode)
                {
                    MessageBox.Show("该图案已存在", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            var myItem = new ListViewItem();
            myItem = lsvBlockSet.Items.Add(blockCode);
            myItem.SubItems.Add(bgColor.ToArgb().ToString());

        }

        private void lsvBlockSet_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                bgColor = Color.FromArgb(int.Parse(e.Item.SubItems[1].Text));
                lblColor.BackColor = bgColor;

                var str = e.Item.SubItems[0].Text;
                for (int i = 0; i < str.Length; i++)
                {
                    struArr[i / 5, i % 5] = str[i] == '1';
                }
                lblMode.Invalidate();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (lsvBlockSet.SelectedItems.Count == 0)
            {
                MessageBox.Show("请在右边窗口选择一个条目进行删除", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lsvBlockSet.Items.Remove(lsvBlockSet.SelectedItems[0]);

            btnClear.PerformClick();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    struArr[x, y] = false;
                }
            }

            lblMode.Invalidate();
        }
    }
}
