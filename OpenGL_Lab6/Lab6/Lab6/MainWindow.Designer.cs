namespace Lab6
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GLControl = new OpenGL.GlControl();
            this.SuspendLayout();
            // 
            // GLControl
            // 
            this.GLControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.GLControl.ColorBits = ((uint)(24u));
            this.GLControl.DebugContext = OpenGL.GlControl.AttributePermission.Enabled;
            this.GLControl.DepthBits = ((uint)(24u));
            this.GLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GLControl.Location = new System.Drawing.Point(0, 0);
            this.GLControl.MultisampleBits = ((uint)(0u));
            this.GLControl.Name = "GLControl";
            this.GLControl.Size = new System.Drawing.Size(800, 450);
            this.GLControl.StencilBits = ((uint)(0u));
            this.GLControl.TabIndex = 0;
            this.GLControl.ContextCreated += new System.EventHandler<OpenGL.GlControlEventArgs>(this.GLControl_ContextCreated);
            this.GLControl.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.GLControl_Render);
            this.GLControl.DragDrop += new System.Windows.Forms.DragEventHandler(this.GLControl_DragDrop);
            this.GLControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GLControl_KeyDown);
            this.GLControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GLControl_MouseDown);
            this.GLControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GLControl_MouseMove);
            this.GLControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GLControl_MouseUp);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.GLControl);
            this.Name = "MainWindow";
            this.Text = "OpenGL";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGL.GlControl GLControl;
    }
}