using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using WenceyWang.FIGlet;

namespace Order
{
    public partial class Form1 : Form
    {
        private Stack<Action> undoStack; // 用于保存撤销操作，压栈
        public Form1()
        {
            InitializeComponent();
            undoStack = new Stack<Action>();
            richTextBox1.Text = (new AsciiArt("Jevon")).ToString();
        }
        //排序
        private void button1_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            var files = Directory.GetFiles(path);//获取文件夹的文件
            var renameHistory = new List<(string oldName, string newName)>();

            int count = 1;
            foreach (var file in files)
            {
                string ext = Path.GetExtension(file);//
                string newFileName = $"{count.ToString("D3")}{ext}";//D表示数字，3表示总位数
                string newFilePath = Path.Combine(path, newFileName);

                File.Move(file, newFilePath);
                renameHistory.Add((file, newFilePath)); // 保存重命名历史
                count++;
            }           
            undoStack.Push(() => UndoRename(renameHistory));

            richTextBox1.Text += "\n文件排序已完成...";
        }
        //更改后缀
        private void button2_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            string ext = "." + textBox2.Text;
            var files = Directory.GetFiles(path);//获取文件夹的文件
            var renameHistory = new List<(string oldName, string newName)>();

            int count = 1;
            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string newFileName = $"{fileName}{ext}";
                string newFilePath = Path.Combine(path, newFileName);

                File.Move(file, newFilePath);
                renameHistory.Add((file, newFilePath)); // 保存重命名历史
                count++;
            }

            undoStack.Push(() => UndoRename(renameHistory));
            richTextBox1.Text += "\n更改后缀已完成...";
        }
        //生成mp4
        private void button3_Click(object sender, EventArgs e)
        {

            string path = textBox1.Text;
            string cmd1 = "ren " + path + "\\*.0 *.mp4";
            string cmd2 = "ren " + path + "\\*.1 *.ts";
            string cmd3 = "ren " + path + "\\*.txt *.mp4";
            Commands(cmd1);
            Commands(cmd2);
            Commands(cmd3);
            richTextBox1.Text += "\n生成MP4已完成...";

        }
        //生成0 ren *.mp4 *.0
        private void button4_Click(object sender, EventArgs e)
        {

            string path = textBox1.Text;
            string cmd1 = "ren " + path + "\\*.mp4 *.0";
            string cmd2 = "ren " + path + "\\*.ts *.1";
            Commands(cmd1);
            Commands(cmd2);
            richTextBox1.Text += "\n生成0已完成...";

        }
        //清空控制台
        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }
        //执行cmd命令
        public void Commands(string cmd)
        {
            // 创建一个新的进程
            Process process = new Process();

            // 配置进程启动信息
            process.StartInfo.FileName = "cmd.exe";         // 指定要运行的程序
            process.StartInfo.Arguments = "/c" + cmd;    // "/c" 表示执行完命令后关闭命令提示符
            process.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            process.StartInfo.UseShellExecute = false;      // 禁用Shell，以便重定向
            process.StartInfo.CreateNoWindow = true;        // 不创建命令提示符窗口

            // 启动进程
            process.Start();

            // 读取输出
            string output = "\n" + process.StandardOutput.ReadToEnd();

            // 等待进程结束
            process.WaitForExit();

            // 输出到控制台
            if (process.StandardOutput.ReadToEnd() != "")
            {
                richTextBox1.Text += output;
            }



        }

        //撤销操作
        private void UndoRename(List<(string oldName, string newName)> renameHistory)
        {
            foreach (var (oldName, newName) in renameHistory)
            {
                File.Move(newName, oldName); // 撤销重命名
            }

            richTextBox1.Text += "\n文件重命名已撤销。";
        }

        public void UndoLastAction()
        {
            if (undoStack.Count > 0)
            {
                var undoAction = undoStack.Pop();
                undoAction.Invoke(); // 执行撤销操作
            }
            else
            {
                richTextBox1.Text += "\n没有操作可以撤销。";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            UndoLastAction();
        }
        //========
    }
}
