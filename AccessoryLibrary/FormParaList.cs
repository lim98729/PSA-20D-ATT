using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DefineLibrary;
using HalconDotNet;

namespace AccessoryLibrary
{
    public partial class FormParaList : Form
    {
        public FormParaList()
        {
            InitializeComponent();

            colName.Name = "colName";
            colValue.Name = "colValue";
            colPreValue.Name = "colPreValue";
            colDefaultValue.Name = "colDefaultValue";
            colLowerLimit.Name = "colLowerLimit";
            colUpperLimit.Name = "colUpperLimit";
            colAuthority.Name = "colAuthority";
            colDescription.Name = "colDescription";
        }

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT lParam);
        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int[] lParam);

        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private int selectedRow, selectedColumn;
        private ListViewClass[] paraObject;

        public HTuple saveTuple;
        private para_member pMember;

        public bool isActivate;

        public void activate(HTuple para)
        {
            try
            {
                if (isActivate) { return; }
                saveTuple = para;

                objectList.Items.Clear();
                ListViewExtendedItem item;
                int paraCount = saveTuple.TupleLength() / 9;// sizeof(para_member);
                paraObject = new ListViewClass[paraCount];

                for (int i = 0; i < paraCount; i++)
                {
                    pMember.name = saveTuple[i * 9 + 0].S;
                    pMember.id = saveTuple[i * 9 + 1].I;
                    pMember.value = saveTuple[i * 9 + 2].D;
                    pMember.preValue = saveTuple[i * 9 + 3].D;
                    pMember.defaultValue = saveTuple[i * 9 + 4].D;
                    pMember.lowerLimit = saveTuple[i * 9 + 5].D;
                    pMember.upperLimit = saveTuple[i * 9 + 6].D;
                    pMember.authority = saveTuple[i * 9 + 7].S;
                    pMember.description = saveTuple[i * 9 + 8].S;

                    paraObject[i] = new ListViewClass(pMember);
                    item = new ListViewExtendedItem(objectList, paraObject[i]);
                    objectList.Items.Add(item);
                }
                isActivate = true;
            }
            catch (Exception)
            {
                isActivate = false;
                MessageBox.Show("ParaSettingPanel Activate Error", "ParaSettingPanel Activate Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void BT_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ClickPoint(Point clickPoint, out int row, out int column)
        {
            const int LVM_GETSUBITEMRECT = 0x1038;		//Is LVM_FIRST (0x1000) + 56
            const int LVM_COLUMNORDERARRAY = 0x103B;	//Is LVM_FIRST (0x1000) + 59
            const int LVIR_BOUNDS = 0;
            bool retval = false;
            RECT subItemRect;
            row = column = -1;
            ListViewItem item = objectList.GetItemAt(clickPoint.X, clickPoint.Y);

            if (item != null && objectList.Columns.Count > 1)
            {
                if (objectList.AllowColumnReorder)
                {
                    int[] columnOrder = new int[objectList.Columns.Count];
                    // Get the order of columns in case they've changed from the user.
                    if (SendMessage(objectList.Handle, LVM_COLUMNORDERARRAY, objectList.Columns.Count, columnOrder) != 0)
                    {
                        int i;
                        // Get the subitem rectangles (except column 0), but get them in the proper order.
                        RECT[] subItemRects = new RECT[objectList.Columns.Count];
                        for (i = 1; i < objectList.Columns.Count; i++)
                        {
                            subItemRects[columnOrder[i]].top = i;
                            subItemRects[columnOrder[i]].left = LVIR_BOUNDS;
                            SendMessage(objectList.Handle, LVM_GETSUBITEMRECT, item.Index, ref subItemRects[columnOrder[i]]);
                        }

                        // Find where column 0 is.
                        for (i = 0; i < columnOrder.Length; i++)
                            if (columnOrder[i] == 0)
                                break;

                        // Fix column 0 since we can't get the rectangle bounds of it using above.
                        if (i > 0)
                        {
                            // If column 0 not at index 0, set using the previous.
                            subItemRects[i].left = subItemRects[i - 1].right;
                            subItemRects[i].right = subItemRects[i].left + objectList.Columns[0].Width;
                        }
                        else
                        {
                            // Else, column 0 is at index 0, so use the next.
                            subItemRects[0].left = subItemRects[1].left - objectList.Columns[0].Width;
                            subItemRects[0].right = subItemRects[1].left;
                        }

                        // Go through the subitem rectangle bounds and see where our point is.
                        for (int index = 0; index < subItemRects.Length; index++)
                        {
                            if (clickPoint.X >= subItemRects[index].left & clickPoint.X <= subItemRects[index].right)
                            {
                                row = item.Index;
                                column = columnOrder[index];
                                retval = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int index = 1; index <= objectList.Columns.Count - 1; index++)
                    {
                        subItemRect = new RECT();
                        subItemRect.top = index;
                        subItemRect.left = LVIR_BOUNDS;
                        if (SendMessage(objectList.Handle, LVM_GETSUBITEMRECT, item.Index, ref subItemRect) != 0)
                        {
                            if (clickPoint.X < subItemRect.left)
                            {
                                row = item.Index;
                                column = 0;
                                retval = true;
                                break;
                            }
                            if (clickPoint.X >= subItemRect.left & clickPoint.X <= subItemRect.right)
                            {
                                row = item.Index;
                                column = index;
                                retval = true;
                                break;
                            }
                        }
                    }
                }
            }
            return retval;
        }

        private void objectList_MouseClick(object sender, MouseEventArgs e)
        {
            if (!isActivate) return;
            if (objectList.SelectedItems.Count == 1)
            {
                Point clickPoint = new Point(e.X, e.Y);
                ClickPoint(clickPoint, out selectedRow, out selectedColumn);

                FormParaSetting ff = new FormParaSetting();
                ff.paraObject = paraObject[selectedRow];
                ff.ShowDialog();

                ((ListViewExtendedItem)objectList.SelectedItems[0]).Update(ff.paraObject);
                //paraObject[selectedRow] = (ListViewClass)ff.paraObject;


                /////////////////////////////////////////
                saveTuple[selectedRow * 9 + 0].S = ff.paraObject.Name;
                //saveTuple[selectedRow * 9 + 1].I = ff.paraObject.id;
                saveTuple[selectedRow * 9 + 2].D = ff.paraObject.Value;
                saveTuple[selectedRow * 9 + 3].D = paraObject[selectedRow].para.preValue;
                //saveTuple[selectedRow * 9 + 4].D = paraObject[selectedRow].para.defaultValue;
                saveTuple[selectedRow * 9 + 5].D = paraObject[selectedRow].para.lowerLimit;
                saveTuple[selectedRow * 9 + 6].D = paraObject[selectedRow].para.upperLimit;
                //saveTuple[selectedRow * 9 + 7].S = paraObject[selectedRow].para.authority;
                //saveTuple[selectedRow * 9 + 8].S = paraObject[selectedRow].para.description;
                ///////////////////////////////////////


            }
        }
    }


    public class ListViewClass
    {
        public para_member para;

        [ListViewColumn("colName")]
        public string Name
        {
            get { return para.name; }
            //set { para.name = value; }
        }
        [ListViewColumn("colValue")]
        public double Value
        {
            get { return para.value; }
            set
            {
                if (value < LowerLimit || value > UpperLimit)
                {
                    MessageBox.Show("Range Error", "Range Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //if (para.preValue != value)
                {
                    para.preValue = para.value;
                }
                para.value = value;
            }
        }
        [ListViewColumn("colPreValue")]
        public double PreValue
        {
            get { return para.preValue; }
            //set { para.defaultValue = value; }
        }
        [ListViewColumn("colDefaultValue")]
        public double DefaultValue
        {
            get { return para.defaultValue; }
            set { para.defaultValue = value; }
        }
        [ListViewColumn("colLowerLimit")]
        public double LowerLimit
        {
            get { return para.lowerLimit; }
            set { para.lowerLimit = value; }
        }
        [ListViewColumn("colUpperLimit")]
        public double UpperLimit
        {
            get { return para.upperLimit; }
            set { para.upperLimit = value; }
        }
        [ListViewColumn("colAuthority")]
        public string Authority
        {
            get { return para.authority; }
            //set { para.authority = value; }
        }
        [ListViewColumn("colDescription")]
        public string Description
        {
            get { return para.description; }
            //set { para.description = value; }
        }

        public ListViewClass(para_member paraMember)
        {
            para = paraMember;
        }
    }
    public class ListViewColumnAttribute : Attribute
    {
        private string _columnName;

        public ListViewColumnAttribute(string columnName)
        {
            _columnName = columnName;
        }

        public override string ToString()
        {
            return _columnName;
        }
    }
    public class ListViewExtendedItem : ListViewItem
    {
        private object _data;

        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        private object _dataBak;

        public object DataBak
        {
            get { return _dataBak; }
            set { _dataBak = value; }
        }

        public ListViewExtendedItem(ListView parentListView, object data)
        {
            Update(data, parentListView);
        }

        public void Update(object data)
        {
            if (this.ListView != null)
            {
                Update(data, this.ListView);
            }
            else
            {
                throw new Exception("The item does not contain a reference of a ListViewControl, please use the provided overload of this method and specify a ListView control");
            }
        }

        public void Update(object data, ListView listView)
        {
            this.SubItems.Clear();
            Type typeOfData = data.GetType();
            bool completed_column = false;
            foreach (ColumnHeader column in listView.Columns)
            {
                completed_column = false;
                foreach (PropertyInfo pInfo in typeOfData.GetProperties())
                {
                    foreach (object pAttrib in pInfo.GetCustomAttributes(true))
                    {
                        if (pAttrib.GetType() == typeof(ListViewColumnAttribute))
                        {
                            if (pAttrib.ToString() == column.Name)
                            {
                                if (column.DisplayIndex == 0)
                                {
                                    this.Text = pInfo.GetValue(data, null).ToString();
                                    completed_column = true;
                                    break;
                                }
                                else
                                {
                                    this.SubItems.Add(pInfo.GetValue(data, null).ToString());
                                    completed_column = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (completed_column)
                    {
                        break;
                    }
                }
            }
            _data = data;
            _dataBak = data;
        }
    }

   
}
