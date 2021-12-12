using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DLLInjector;

public class Form1 : Form
{
	private IContainer components = null;

	private Label label1;

	private ComboBox procComboBox;

	private Label label2;

	private Button Inject;

	private OpenFileDialog openDLL;

	private TextBox textBox1;


	private Button AddDllButton;

	public Form1()
	{
		InitializeComponent();
	}

	[DllImport("kernel32")]
	public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

	[DllImport("kernel32.dll")]
	public static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, int dwProcessId);

	[DllImport("kernel32.dll")]
	public static extern int CloseHandle(IntPtr hObject);

	[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
	private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint dwFreeType);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
	public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

	[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
	private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

	[DllImport("kernel32.dll")]
	private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, string lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr GetModuleHandle(string lpModuleName);

	[DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
	internal static extern int WaitForSingleObject(IntPtr handle, int milliseconds);

	public int GetProcessId(string proc)
	{
		Process[] processesByName = Process.GetProcessesByName(proc);
		return processesByName[0].Id;
	}

	public unsafe void InjectDLL(IntPtr hProcess, string strDLLName)
	{
		int num = strDLLName.Length + 1;
		IntPtr intPtr = VirtualAllocEx(hProcess, (IntPtr)(void*)null, (uint)num, 4096u, 64u);
		WriteProcessMemory(hProcess, intPtr, strDLLName, (UIntPtr)(ulong)num, out var lpNumberOfBytesWritten);
		UIntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
		bool flag = 0 == 0;
		IntPtr intPtr2 = CreateRemoteThread(hProcess, (IntPtr)(void*)null, 0u, procAddress, intPtr, 0u, out lpNumberOfBytesWritten);
		flag = 0 == 0;
		int num2 = WaitForSingleObject(intPtr2, 10000);
		if ((long)num2 == 128 || (long)num2 == 258 || num2 == uint.MaxValue)
		{
			MessageBox.Show(" hThread [ 2 ] Error! \n ");
			flag = 1 == 0;
			CloseHandle(intPtr2);
		}
		else
		{
			Thread.Sleep(1000);
			VirtualFreeEx(hProcess, intPtr, (UIntPtr)0u, 32768u);
			flag = 1 == 0;
			CloseHandle(intPtr2);
		}
	}

	private void AddDllButton_Click(object sender, EventArgs e)
	{
		DialogResult dialogResult = openDLL.ShowDialog();
		if (dialogResult == DialogResult.OK)
		{
			string fileName = Path.GetFileName(openDLL.FileName);
			textBox1.Text = fileName;
		}
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		Process[] processes = Process.GetProcesses();
		Process[] array = processes;
		foreach (Process process in array)
		{
			procComboBox.Items.Add(process.ProcessName);
			procComboBox.Sorted = true;
			procComboBox.SelectedIndex = 0;
		}
	}

	private void Inject_Click(object sender, EventArgs e)
	{
		string fileName = openDLL.FileName;
		if (procComboBox.SelectedIndex == -1)
		{
			MessageBox.Show("No process selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		string text = procComboBox.SelectedItem.ToString();
        int processId = GetProcessId(text);
		
		if (procComboBox.SelectedIndex == -1)
		{

			MessageBox.Show("no dll selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}

		if (processId >= 0)
		{
			IntPtr hProcess = OpenProcess(2035711u, 1, processId);
			bool flag = 0 == 0;
			string fileName2 = Path.GetFileName(openDLL.FileName);

			

			DialogResult dialogResult = MessageBox.Show("You are injecting the DLL '" + fileName2 + "' into the process '" + text + "' continue?", "Inject", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
			if (dialogResult == DialogResult.Yes)
			{
				InjectDLL(hProcess, fileName);
				MessageBox.Show("Injection successful!, DLL Injector will now close.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				Application.Exit();
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
            this.label1 = new System.Windows.Forms.Label();
            this.procComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Inject = new System.Windows.Forms.Button();
            this.openDLL = new System.Windows.Forms.OpenFileDialog();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.AddDllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Process:";
            // 
            // procComboBox
            // 
            this.procComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.procComboBox.FormattingEnabled = true;
            this.procComboBox.Location = new System.Drawing.Point(128, 29);
            this.procComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.procComboBox.Name = "procComboBox";
            this.procComboBox.Size = new System.Drawing.Size(433, 28);
            this.procComboBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 101);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 29);
            this.label2.TabIndex = 2;
            this.label2.Text = "DLL:";
            // 
            // Inject
            // 
            this.Inject.Location = new System.Drawing.Point(20, 186);
            this.Inject.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Inject.Name = "Inject";
            this.Inject.Size = new System.Drawing.Size(548, 72);
            this.Inject.TabIndex = 6;
            this.Inject.Text = "Inject";
            this.Inject.UseVisualStyleBackColor = true;
            this.Inject.Click += new System.EventHandler(this.Inject_Click);
            // 
            // openDLL
            // 
            this.openDLL.Filter = "DLL Files|*.dll";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(128, 105);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(380, 26);
            this.textBox1.TabIndex = 7;
            // 
            // AddDllButton
            // 
            this.AddDllButton.Location = new System.Drawing.Point(524, 105);
            this.AddDllButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AddDllButton.Name = "AddDllButton";
            this.AddDllButton.Size = new System.Drawing.Size(44, 31);
            this.AddDllButton.TabIndex = 4;
            this.AddDllButton.Text = "...";
            this.AddDllButton.UseVisualStyleBackColor = true;
            this.AddDllButton.Click += new System.EventHandler(this.AddDllButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(711, 422);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Inject);
            this.Controls.Add(this.AddDllButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.procComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.RightToLeftLayout = true;
            this.Text = "DLL Injector";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
