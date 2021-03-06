using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CountDown_Day
{
    //用於ComboBox
    public class drop_down_list
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int IDS { get; set; }
    }
    //倒數日記錄, ID用於傳參
    public class countdown_schedule
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime time { get; set; }
        public bool isshow { get; set; }
        public string filename { get; set; }
    }
    //按鈕記錄, ID用於傳參
    public class button_map
    {
        public int ID { get; set; }
        public Button button { get; set; }
    }
    public sealed partial class MainPage : Page
    {
        public static List<countdown_schedule> schedules = new List<countdown_schedule>();
        public static List<button_map> buttonmaps = new List<button_map>();
        int nowid = 0;
        int len = (int)((Window.Current.Bounds.Height - 104) / 48);
        public enum ForeDate
        {
            Past = 1,
            Future = -1,
            Now = 0
        }
        /// <summary>
        /// 用於獲取指定日期與當天的舌關係
        /// </summary>
        /// <param name="dateTime">指定日期</param>
        public static ForeDate GetForeDate(DateTime dateTime)
        {
            int i = DateTime.Now.CompareTo(dateTime);
            if (i == 0)
                return ForeDate.Now;
            else if (i < 0)
                return ForeDate.Future;
            else if (i > 0)
                return ForeDate.Past;
            else
                throw new Exception();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UIElement uIElement = this.IFrame;
            if (!File.Exists(localfolder.Path + "\\config.ini"))
            {
                //嘗試創建文件並顯示「空」選項
                try
                {
                    localfolder.CreateFileAsync("config.ini", CreationCollisionOption.FailIfExists);
                }
                catch (Exception ex)
                {
                    var dialog = new MessageDialog("Cannot Create File:" + localfolder.Path + "\\config.ini\n" + ex.ToString(), ex.Message);
                    dialog.Options = MessageDialogOptions.AcceptUserInputAfterDelay;
                    dialog.ShowAsync();
                }
                this.IEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                /*if (FileExtend.IsEmpty(localfolder.Path + "\\config.ini"))
                {
                    this.IEmpty.Visibility = Visibility.Visible;
                }
                else
                {
                    string[] schedule = File.ReadAllLines(localfolder.Path + "\\config.ini");
                    int i = 0;
                    foreach (var v in schedule)
                    {
                        string[] args = v.Split(new char[] { ' ', ':' });
                        DateTime targetdt = new DateTime(Convert.ToInt32(args[0]) == 0 ? (GetForeDate(new DateTime(DateTime.Now.Year, Convert.ToInt32(args[1]), Convert.ToInt32(args[2]))) == ForeDate.Future ? DateTime.Now.Year : DateTime.Now.Year + 1) : Convert.ToInt32(args[0]), Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
                        schedules.Add(new countdown_schedule { ID = i, isshow = false, Name = args[3], time = targetdt });
                        double h = Window.Current.Bounds.Height - 48.0;
                        if (((i + 1) * 48 + 8) < h)
                        {
                            buttonmaps.Add(new button_map { ID = i, button = new Button() });
                            buttonmaps[i].button.Height = 32;
                            buttonmaps[i].button.Margin = new Thickness(8, i * 40 + 8, 8, 0);
                            buttonmaps[i].button.Content = targetdt.ToShortDateString() + "\t" + args[3];
                            buttonmaps[i].button.Visibility = Visibility.Visible;
                            buttonmaps[i].button.VerticalAlignment = VerticalAlignment.Top;
                            buttonmaps[i].button.Click += Upd_Schedule;
                            buttonmaps[i].button.RightTapped += Change_Schedule;
                            this.IFrame.Children.Add(buttonmaps[i].button);
                        }
                        i++;
                    }
                    
                }*/
                DirectoryInfo di = new DirectoryInfo(localfolder.Path);
                int i6 = 0;
                foreach (var v in di.GetFiles())
                {
                    if (v.Name.Contains("config.ini"))
                        continue;
                    string fname = v.FullName;
                    string[] values;
                    try
                    {
                        values = File.ReadAllLines(fname);
                    }
                    catch (Exception ex)
                    {
                        MessageDialog dialog = new MessageDialog(ex.ToString(), ex.Message);
                        return;
                    }
                    int year = 0, month = 0, day = 0;
                    string tl = "";
                    int yst = 0;
                    bool isst = false;
                    foreach (var u in values)
                    {
                        string[] c = u.Split('=');
                        for (int i = 0; i < c.Length; i++)
                        {
                            c[i] = c[i].Trim();
                        }
                        if (c[0] == "Month")
                        {
                            month = Convert.ToInt32(c[1]);
                        }
                        else if (c[0] == "Day")
                        {
                            day = Convert.ToInt32(c[1]);
                        }
                        else if (c[0] == "Title")
                        {
                            tl = c[1];
                        }
                        if (c[0] == "Year")
                        {
                            if (((month == 0) || (day == 0)) && (Convert.ToInt32(c[1]) == 0))
                            {
                                isst = true;
                                yst = Convert.ToInt32(c[1]);
                            }
                            else if (Convert.ToInt32(c[1]) != 0)
                            {
                                year = Convert.ToInt32(c[1]);
                            }
                            else
                            {
                                year = MainPage.GetForeDate(new DateTime(DateTime.Now.Year, month, day)) == ForeDate.Future ? DateTime.Now.Year : DateTime.Now.Year + 1;
                            }
                        }
                    }
                    if (isst)
                    {
                        year = yst == 0 ? (MainPage.GetForeDate(new DateTime(DateTime.Now.Year, month, day)) == ForeDate.Future ? DateTime.Now.Year : DateTime.Now.Year + 1) : yst;
                    }
                    DateTime targetdt = new DateTime(year, month, day);
                    schedules.Add(new countdown_schedule { ID = i6, isshow = false, Name = tl, time = targetdt, filename = fname });
                    i6++;
                }
                this.ReLoadItems();
            }
        }
        //用於修改/添加/刪除倒數日後的重載按鈕
        //用法: App.Main?.ReLoadItems()
        public async void ReLoadItems(int status = 0)
        {
            buttonmaps.Clear();
            this.IFrame.Children.Clear();
            int i = status == 0 ? 0 : status;
            // MessageDialog dialog = new MessageDialog("ReloadItems Detached.");
            // dialog.ShowAsync();
            if (schedules.Count == 0)
            {
                this.IEmpty.Visibility = Visibility.Visible;
                return;
            }
            else
                this.IEmpty.Visibility = Visibility.Collapsed;
            double h = Window.Current.Bounds.Height - 48.0;
            foreach (var v in schedules)
            {
                if (v.ID < status)
                    continue;
                if ((((i - status) + 1) * 48 + 8) < h - 48)
                {
                    buttonmaps.Add(new button_map { ID = i, button = new Button() });
                    buttonmaps[i - status].button.Height = 32;
                    buttonmaps[i - status].button.Margin = new Thickness(8, (i - status) * 40 + 8, 8, 0);
                    buttonmaps[i - status].button.Content = v.time.ToShortDateString() + "\t" + v.Name;
                    buttonmaps[i - status].button.Visibility = Visibility.Visible;
                    buttonmaps[i - status].button.VerticalAlignment = VerticalAlignment.Top;
                    buttonmaps[i - status].button.Click += Upd_Schedule;
                    buttonmaps[i - status].button.RightTapped += Change_Schedule;
                    IFrame.Children.Add(buttonmaps[i - status].button);
                }
                else if((i - 1) > nowid)
                {
                    nowid = i;
                    break;
                }
                i++;
            }
            if (nowid != i)
                nowid = 0;
            this.TPage.Text = Convert.ToString((schedules.Count / len) - (nowid / len) + 1) + "/" + Convert.ToString(schedules.Count / len + 1);
        }
    }
}