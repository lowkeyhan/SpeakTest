using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace speaktest
{
    public partial class Form1 : Form
    {
        // *********************************************  全局变量  **********************************************************************   
        SpeechRecognitionEngine _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("zh-CN")); // 语音识别引擎
        SpeechSynthesizer speech = new SpeechSynthesizer();  // 语音合成器。默认使用女声Lily。
        
     

        bool IsStandingBy = false;   // 是否进入待命模式
        bool Issenced = false;
        System.Threading.Timer tmrCurrent;   // 当前的
        System.Threading.Timer tmrPrevious;  //  234 之前的



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            
            speech.SelectVoiceByHints(VoiceGender.Male,VoiceAge.Adult);
            // 添加语法
            GrammarBuilder gb1 = new GrammarBuilder(new Choices("小白", "取消"));
            GrammarBuilder gb2 = new GrammarBuilder(new Choices("好吃吗", "好吃"));
            GrammarBuilder gb3 = new GrammarBuilder(new Choices("还想再吃", "还吃吗", "18度"));
            GrammarBuilder gb4 = new GrammarBuilder(new Choices("打开房门"));
            GrammarBuilder gb5 = new GrammarBuilder(new Choices("吃什么", "吃屁"));
            // 加载语法
            _recognizer.LoadGrammar(new Grammar(gb1));
            _recognizer.LoadGrammar(new Grammar(gb2));
            _recognizer.LoadGrammar(new Grammar(gb3));
            _recognizer.LoadGrammar(new Grammar(gb4));
            _recognizer.LoadGrammar(new Grammar(gb5));
            // 绑定事件
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);

            _recognizer.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(_recognizer_SpeechRecognitionRejected);

            _recognizer.SetInputToDefaultAudioDevice();   // 设置语音输入设备
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // 开启异步语音识别

        }
        /// <summary>
        /// 语音识别后的处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // 关闭识别，防止speech说出来的话被误识别
            _recognizer.RecognizeAsyncStop();
            Thread.Sleep(30);
            Console.WriteLine("测试");
            if (e.Result.Confidence > 0.95)
            {
                Console.WriteLine(e.Result.Text);

                if (e.Result.Text == "计算机" || e.Result.Text == "小白" || Issenced==true)
                {
                    if (!Issenced)
                    {
                        speech.Speak("在");
                    }
                    // 进入待命模式
                    IsStandingBy = true;
                    Issenced = true;
                    Console.WriteLine("进入待命模式");
                    // 重新计时，10秒命令超时。
                    tmrPrevious = tmrCurrent;
                    tmrCurrent = new System.Threading.Timer(new TimerCallback(TimerCall), this, 10000, 0);  // 当前的，新的
                    if (tmrPrevious != null) // 取消前一个tmr.
                    {
                        tmrPrevious.Dispose();
                    }
                }

                // 待命模式下的处理 
                if (IsStandingBy == true && e.Result.Text != "小白")
                {
                    if (e.Result.Text == "吃什么")
                    {
                        //lightController.TurnOnLight1();
                        speech.Speak("吃屁");
                    }
                    else if (e.Result.Text == "好吃吗")
                    {
                        //lightController.TurnOffLight1();
                        speech.Speak("好吃");
                    }
                    else if (e.Result.Text == "还吃吗")
                    {
                        //lightController.TurnOffLight1();
                        speech.Speak("还想再吃");
                    }
                    //else
                    //{
                     //   speech.Speak("请提出正确的问题");
                    //}
                    IsStandingBy = false; // 退出待命模式
                    tmrCurrent.Dispose();
                    Console.WriteLine("退出待命模式");
                }
            }

            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // 开启识别
        }


        void _recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            _recognizer.RecognizeAsyncStop();
            Thread.Sleep(30);
            speech.Speak("请再说一遍");
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // 开启识别
        }



        void TimerCall(object sender)
        {
            IsStandingBy = false;
            Console.WriteLine("退出待命模式");
        }




    }
}
