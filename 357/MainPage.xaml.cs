using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace _357
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int WhoDo = 0;//0-玩家A ,1-AI ，2-玩家B
        int[] GameBoard = { 0, 0, 0 };
        int LastAddUp = 0;
        public bool isAI = true;
        public MainPage()
        {
            this.InitializeComponent();
            extendAcrylicIntoTitleBar();
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size { Width = 500, Height = 280 });

            ApplicationView.PreferredLaunchViewSize = new Size(700, 320);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            SetButton123(false, false, false);
            SetPState(true, true, true);
            Reset();
        }

        private void extendAcrylicIntoTitleBar()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar; titleBar.ButtonBackgroundColor = Colors.Transparent; titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        void SetPState(bool first, bool second, bool third)
        {
            if (first == true)
            {
                P1.Visibility = Visibility.Visible;
            }
            else
            {
                P1.Visibility = Visibility.Collapsed;
            }
            if (second == true)
            {
                P2.Visibility = Visibility.Visible;
            }
            else
            {
                P2.Visibility = Visibility.Collapsed;
            }
            if (third == true)
            {
                P3.Visibility = Visibility.Visible;
            }
            else
            {
                P3.Visibility = Visibility.Collapsed;
            }
        }

        void SetButton123(bool first, bool second, bool third)
        {
            FirstButton.IsEnabled = first;
            SecondButton.IsEnabled = second;
            ThirdButton.IsEnabled = third;
        }

        void UserSelect()//打开保护膜，打开板块选择按钮
        {
            ConfirmButton.IsEnabled = true;
            SetButton123(true, true, true);
            //打开保护膜123
            SetPState(true, true, true);
        }

        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            SetPState(false, true, true);
            //隐藏1叠加层
            //显示2叠加层
            //显示3叠加层
            SecondButton.IsChecked = false;
            ThirdButton.IsChecked = false;
        }

        private void SecondButton_Click(object sender, RoutedEventArgs e)
        {
            SetPState(true, false, true);
            //显示1叠加层
            //隐藏2叠加层
            //显示3叠加层
            FirstButton.IsChecked = false;
            ThirdButton.IsChecked = false;
        }

        private void ThirdButton_Click(object sender, RoutedEventArgs e)
        {
            SetPState(true, true, false);
            //显示1叠加层
            //显示2叠加层
            //隐藏3叠加层
            FirstButton.IsChecked = false;
            SecondButton.IsChecked = false;
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerAIButton.IsChecked == false && PlayerBButton.IsChecked == false)
            {
                ShowMessageDialog("在右上角选择对战对象后才能开新局", "你要与谁玩？");
            }
            else
            {
                LastAddUp = AddUp();//记录数据

                //选择了对战对象
                PlayerAButton.Visibility = Visibility.Visible;//显示玩家A
                PlayerP.Visibility = Visibility.Visible;//打开叠加层
                NewGameButton.IsEnabled = false;//禁用新开局按钮
                EndGameButton.IsEnabled = true;//开启停止游戏按钮
                if (PlayerAIButton.IsChecked == true)
                {
                    isAI = true;
                    PlayerBButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    isAI = false;
                    PlayerAIButton.Visibility = Visibility.Collapsed;
                }

                //重置控件
                UserSelect();
                Reset();

                if (PlayerAIButton.IsChecked == true)//随机玩家开始
                {
                    //AI
                    WhoDo = RandomMath(0, 1);
                    isAI = true;
                    if (WhoDo == 1)
                    {
                        MachineRadom();//低级AI！！
                        UpdateSelector();
                        WhoDo = 0;
                    }
                }
                else
                {
                    //玩家B
                    if (RandomMath(0, 1) == 1)//如果随机结果是玩家B
                    {
                        WhoDo = 2;//玩家B
                    }
                    else
                    {
                        WhoDo = 0; //玩家A
                    }
                    isAI = false;
                }
                UpdateSelector();
                //开始新局
            }
        }

        bool Judger()//返回值代表游戏是否结束
        {
            if (AddUp() < 15)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        int RandomMath(int from, int to)
        {
            Random rd = new Random();
            return rd.Next(from, to + 1);//(生成1~10之间的随机数，包括to,from)
        }

        void ToggleButtonAmountCounter(ToggleButton button, int block)//只检测一个，直接加入数字
        {
            if (button.IsEnabled == false)//扫描已按下的按钮
            {
                switch (block)
                {
                    case 1:
                        GameBoard[0] += 1;
                        break;
                    case 2:
                        GameBoard[1] += 1;
                        break;
                    case 3:
                        GameBoard[2] += 1;
                        break;
                }
                button.IsEnabled = false;
            }
        }

        void confirm()//确认总数值，暴力扫描
        {
            GameBoard[0] = 0; GameBoard[1] = 0; GameBoard[2] = 0;//GameBoard全部清零

            //P1已打开
            ToggleButtonAmountCounter(G101, 1);
            ToggleButtonAmountCounter(G102, 1);
            ToggleButtonAmountCounter(G103, 1);

            //P2已打开
            ToggleButtonAmountCounter(G201, 2);
            ToggleButtonAmountCounter(G202, 2);
            ToggleButtonAmountCounter(G203, 2);
            ToggleButtonAmountCounter(G204, 2);
            ToggleButtonAmountCounter(G205, 2);

            //P3已打开
            ToggleButtonAmountCounter(G301, 3);
            ToggleButtonAmountCounter(G302, 3);
            ToggleButtonAmountCounter(G303, 3);
            ToggleButtonAmountCounter(G304, 3);
            ToggleButtonAmountCounter(G305, 3);
            ToggleButtonAmountCounter(G306, 3);
            ToggleButtonAmountCounter(G307, 3);

            SetPState(true, true, true);
        }

        int AddUp()
        {
            confirm();
            return GameBoard[0] + GameBoard[1] + GameBoard[2];
        }

        void MachineRadom()
        {
            confirm();
            int block = 0, math = 0;//初始化变量:板块，选择个数
            re://返回点，重新随机数
            block = RandomMath(0, 2);//(0,1,2)三个编码block
            switch (block)
            {
                case 0:
                    //一共有3个,剩下的可以随机
                    if (GameBoard[0] != 3)//还有剩余
                    {
                        /**
                         * 如果还有剩余，那么至少还有1个
                         * 如下会从1开始取随机数，到剩余数(最少1)
                         * **/
                        int random = RandomMath(1, 3 - GameBoard[0]);
                        DoConfirmController(0, random);//从1到剩下的数
                    }
                    else
                    {
                        //无剩余
                        goto re;
                    }
                    break;
                case 1:
                    //一共有5个,剩下的可以随机
                    if (GameBoard[1] != 5)//还有剩余
                    {
                        int random = RandomMath(1, 5 - GameBoard[1]);
                        DoConfirmController(1, random);//从1到剩下的数
                    }
                    else
                    {
                        goto re;
                    }
                    break;
                case 2:
                    //一共有5个,剩下的可以随机
                    if (GameBoard[2] != 7)//还有剩余
                    {
                        int random = RandomMath(1, 7 - GameBoard[2]);
                        DoConfirmController(2, random);//从1到剩下的数
                    }
                    else
                    {
                        goto re;
                    }
                    break;
            }
            //低等ai处理完毕
        }

        void DoConfirmController(int block, int math)//板块编号，消除数（自带AI判别）
        {
            ToggleButton[] first = { G101, G102, G103 };
            ToggleButton[] second = { G201, G202, G203, G204, G205 };
            ToggleButton[] third = { G301, G302, G303, G304, G305, G306, G307 };

            switch (block)//执行针对板块按下按钮
            {
                case 0:
                    while (math != 0)//一直找按钮按下，直到math被减到0
                    {
                        re1:
                        int i = RandomMath(0, 2);//按下按钮选择器,随机
                        if (first[i].IsEnabled == true)
                        {
                            first[i].IsEnabled = false;
                        }
                        else
                        {
                            goto re1;//重新生成
                        }
                        math -= 1;
                    }
                    break;
                case 1:
                    while (math != 0)//一直找按钮按下，直到math被减到0
                    {
                        re1:
                        int i = RandomMath(0, 4);//按下按钮选择器,随机
                        if (second[i].IsEnabled == true)
                        {
                            second[i].IsEnabled = false;
                        }
                        else
                        {
                            goto re1;//重新生成
                        }
                        math -= 1;
                    }
                    break;
                case 2:
                    while (math != 0)//一直找按钮按下，直到math被减到0
                    {
                        re1:
                        int i = RandomMath(0, 6);//按下按钮选择器,随机
                        if (third[i].IsEnabled == true)
                        {
                            third[i].IsEnabled = false;
                        }
                        else
                        {
                            goto re1;//重新生成
                        }
                        math -= 1;
                    }
                    break;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddUp() == LastAddUp)//如果没走棋
            {
                ShowMessageDialog("您还没有消去任何方块，不能确认。", "提示");
            }
            else
            {
                LastAddUp = AddUp();//走棋了，更新数据

                FirstButton.IsChecked = false;
                SecondButton.IsChecked = false;
                ThirdButton.IsChecked = false;
                SetButton123(true, true, true);

                confirm();
                if (Judger())
                {
                    //游戏未结束
                    UserSelect();
                    if (WhoDo == 0)//玩家A按下确认
                    {
                        if (isAI == true)
                        {
                            //算法
                            //更新界面
                            WhoDo = 1;
                            UpdateSelector();
                            //初级算法
                            MachineRadom();
                            confirm();
                            //玩家会再次点此按钮
                        }
                        else
                        {
                            //玩家B
                            WhoDo = 2;
                            UpdateSelector();
                            //玩家B将点此按钮
                        }
                    }
                    else
                    {
                        //非玩家A按下确认
                        WhoDo = 0;//(risk of AI)
                        UpdateSelector();
                    }
                }
                if (!Judger())
                {
                    //游戏已结束，判别（Toast）
                    if (WhoDo != 0)
                    {
                        ShowMessageDialog("恭喜玩家A获胜", "游戏结束");//玩家A获胜
                    }
                    else if (isAI == true)
                    {
                        ShowMessageDialog("很遗憾，您未能获胜", "游戏结束");//AI获胜
                    }
                    else
                    {
                        ShowMessageDialog("恭喜玩家B获胜", "游戏结束");//玩家B获胜
                    }
                    SetButton123(false, false, false);//禁用按钮
                    ConfirmButton.IsEnabled = false;//禁用确认按钮
                }
                if (isAI && WhoDo == 1)
                {
                    WhoDo = 0;
                    UpdateSelector();
                }
            }
        }

        private async void ShowMessageDialog(string msg, string title)
        {
            var msgDialog = new Windows.UI.Popups.MessageDialog(msg) { Title = title };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定"));
            await msgDialog.ShowAsync();
        }

        private void PlayerAIButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerBButton.IsChecked = false;
        }

        private void PlayerBButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerAIButton.IsChecked = false;
        }

        void Reset()//清除器
        {
            ToggleButton[] first = { G101, G102, G103 };
            ToggleButton[] second = { G201, G202, G203, G204, G205 };
            ToggleButton[] third = { G301, G302, G303, G304, G305, G306, G307 };

            foreach (ToggleButton t in first)
            {
                t.IsEnabled = true;
                t.IsChecked = true;
            }
            foreach (ToggleButton t in second)
            {
                t.IsEnabled = true;
                t.IsChecked = true;
            }
            foreach (ToggleButton t in third)
            {
                t.IsEnabled = true;
                t.IsChecked = true;
            }
        }

        void UpdateSelector()
        {
            switch (WhoDo)
            {
                case 0:
                    PlayerAButton.IsChecked = true;
                    PlayerAIButton.IsChecked = false;
                    PlayerBButton.IsChecked = false;
                    break;
                case 1:
                    PlayerAButton.IsChecked = false;
                    PlayerAIButton.IsChecked = true;
                    PlayerBButton.IsChecked = false;
                    break;
                case 2:
                    PlayerAButton.IsChecked = false;
                    PlayerAIButton.IsChecked = false;
                    PlayerBButton.IsChecked = true;
                    break;
            }
        }

        private void EndGameButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerP.Visibility = Visibility.Collapsed;//隐藏叠加层
            PlayerAButton.Visibility = Visibility.Collapsed;//隐藏玩家A的选项
            PlayerAIButton.Visibility = Visibility.Visible;//显示AI的选项
            PlayerBButton.Visibility = Visibility.Visible;//显示玩家B的选项
            EndGameButton.IsEnabled = false;//禁用停止游戏按钮
            NewGameButton.IsEnabled = true;//开启新开局按钮
            Reset();
        }

        private void G101_Click(object sender, RoutedEventArgs e)
        {
            G101.IsEnabled = false;
            SecondButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G102_Click(object sender, RoutedEventArgs e)
        {
            G102.IsEnabled = false;
            SecondButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G103_Click(object sender, RoutedEventArgs e)
        {
            G103.IsEnabled = false;
            SecondButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G201_Click(object sender, RoutedEventArgs e)
        {
            G201.IsEnabled = false;
            FirstButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G202_Click(object sender, RoutedEventArgs e)
        {
            G202.IsEnabled = false;
            FirstButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G203_Click(object sender, RoutedEventArgs e)
        {
            G203.IsEnabled = false;
            FirstButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G204_Click(object sender, RoutedEventArgs e)
        {
            G204.IsEnabled = false;
            FirstButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G205_Click(object sender, RoutedEventArgs e)
        {
            G205.IsEnabled = false;
            FirstButton.IsEnabled = false;
            ThirdButton.IsEnabled = false;
        }

        private void G301_Click(object sender, RoutedEventArgs e)
        {
            G301.IsEnabled = false;
            FirstButton.IsEnabled = false;
            SecondButton.IsEnabled = false;
        }

        private void G302_Click(object sender, RoutedEventArgs e)
        {
            G302.IsEnabled = false;
            FirstButton.IsEnabled = false;
            SecondButton.IsEnabled = false;
        }

        private void G303_Click(object sender, RoutedEventArgs e)
        {
            G303.IsEnabled = false;
            FirstButton.IsEnabled = false;
            SecondButton.IsEnabled = false;
        }

        private void G304_Click(object sender, RoutedEventArgs e)
        {
            G304.IsEnabled = false;
            FirstButton.IsEnabled = false;
            SecondButton.IsEnabled = false;
        }

        private void G305_Click(object sender, RoutedEventArgs e)
        {
            G305.IsEnabled = false;
            FirstButton.IsEnabled = false;
            SecondButton.IsEnabled = false;
        }

        private void G306_Click(object sender, RoutedEventArgs e)
        {
            G306.IsEnabled = false;
            FirstButton.IsEnabled = false;
            SecondButton.IsEnabled = false;
        }

        private void G307_Click(object sender, RoutedEventArgs e)
        {
            G307.IsEnabled = false;
            FirstButton.IsEnabled = false;
            SecondButton.IsEnabled = false;
        }

        private async void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new Instructions();
            await dialog.ShowAsync();
        }
    }
}
