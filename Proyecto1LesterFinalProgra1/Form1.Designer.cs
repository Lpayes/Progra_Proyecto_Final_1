namespace Proyecto1LesterFinalProgra
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            txtPrompt = new TextBox();
            btnConsultar = new Button();
            txtRespuesta = new TextBox();
            labelPrompt = new Label();
            labelRespuesta = new Label();
            btnLimpiar = new Button();
            btnGuardar = new Button();
            txtPromptPersonalizado = new TextBox();
            labelPromptEstructurado = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // txtPrompt
            // 
            txtPrompt.Location = new Point(383, 197);
            txtPrompt.Multiline = true;
            txtPrompt.Name = "txtPrompt";
            txtPrompt.ScrollBars = ScrollBars.Vertical;
            txtPrompt.Size = new Size(569, 56);
            txtPrompt.TabIndex = 0;
            // 
            // btnConsultar
            // 
            btnConsultar.BackColor = Color.OrangeRed;
            btnConsultar.Font = new Font("Arial", 10.2F);
            btnConsultar.Location = new Point(478, 636);
            btnConsultar.Name = "btnConsultar";
            btnConsultar.Size = new Size(120, 40);
            btnConsultar.TabIndex = 1;
            btnConsultar.Text = "Consultar AI";
            btnConsultar.UseCompatibleTextRendering = true;
            btnConsultar.UseVisualStyleBackColor = false;
            btnConsultar.Click += btnConsultar_Click;
            // 
            // txtRespuesta
            // 
            txtRespuesta.Location = new Point(383, 413);
            txtRespuesta.Multiline = true;
            txtRespuesta.Name = "txtRespuesta";
            txtRespuesta.ReadOnly = true;
            txtRespuesta.ScrollBars = ScrollBars.Vertical;
            txtRespuesta.Size = new Size(569, 200);
            txtRespuesta.TabIndex = 2;
            // 
            // labelPrompt
            // 
            labelPrompt.AutoSize = true;
            labelPrompt.BackColor = Color.OrangeRed;
            labelPrompt.Font = new Font("Arial", 10.2F);
            labelPrompt.Location = new Point(40, 213);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(279, 38);
            labelPrompt.TabIndex = 3;
            labelPrompt.Text = "Ingrese el titulo del tema a investigar\r\n(Ya hay un promt que recibe el titulo)";
            // 
            // labelRespuesta
            // 
            labelRespuesta.AutoSize = true;
            labelRespuesta.BackColor = Color.OrangeRed;
            labelRespuesta.Font = new Font("Arial", 10.2F);
            labelRespuesta.Location = new Point(40, 416);
            labelRespuesta.Name = "labelRespuesta";
            labelRespuesta.Size = new Size(115, 19);
            labelRespuesta.TabIndex = 4;
            labelRespuesta.Text = "Respuesta AI: ";
            // 
            // btnLimpiar
            // 
            btnLimpiar.BackColor = Color.OrangeRed;
            btnLimpiar.Font = new Font("Arial", 10.2F);
            btnLimpiar.Location = new Point(730, 636);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(120, 40);
            btnLimpiar.TabIndex = 6;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = false;
            btnLimpiar.Click += btnLimpiar_Click;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.OrangeRed;
            btnGuardar.Font = new Font("Arial", 10.2F);
            btnGuardar.Location = new Point(604, 636);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(120, 40);
            btnGuardar.TabIndex = 8;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseCompatibleTextRendering = true;
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // txtPromptPersonalizado
            // 
            txtPromptPersonalizado.Location = new Point(383, 293);
            txtPromptPersonalizado.Multiline = true;
            txtPromptPersonalizado.Name = "txtPromptPersonalizado";
            txtPromptPersonalizado.ScrollBars = ScrollBars.Vertical;
            txtPromptPersonalizado.Size = new Size(569, 80);
            txtPromptPersonalizado.TabIndex = 9;
            // 
            // labelPromptEstructurado
            // 
            labelPromptEstructurado.AutoSize = true;
            labelPromptEstructurado.BackColor = Color.OrangeRed;
            labelPromptEstructurado.Font = new Font("Arial", 10.2F);
            labelPromptEstructurado.Location = new Point(40, 308);
            labelPromptEstructurado.Name = "labelPromptEstructurado";
            labelPromptEstructurado.Size = new Size(310, 19);
            labelPromptEstructurado.TabIndex = 10;
            labelPromptEstructurado.Text = "Ingresa Prompt Personalizado (Opcional)";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Tan;
            label1.Font = new Font("Palatino Linotype", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(40, 18);
            label1.Name = "label1";
            label1.Size = new Size(912, 138);
            label1.TabIndex = 11;
            label1.Text = "Bienvenido al Generador de Documentos Académicos IA \r\nde Lester Payes\r\n\r\n";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Highlight;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(991, 699);
            Controls.Add(label1);
            Controls.Add(labelPromptEstructurado);
            Controls.Add(txtPromptPersonalizado);
            Controls.Add(btnGuardar);
            Controls.Add(btnLimpiar);
            Controls.Add(labelRespuesta);
            Controls.Add(labelPrompt);
            Controls.Add(txtRespuesta);
            Controls.Add(btnConsultar);
            Controls.Add(txtPrompt);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            ShowIcon = false;
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtPrompt;
        private Button btnConsultar;
        private TextBox txtRespuesta;
        private Label labelPrompt;
        private Label labelRespuesta;
        private Button btnLimpiar;
        private Button btnGuardar;
        private TextBox txtPromptPersonalizado;
        private Label labelPromptEstructurado;
        private Label label1;
    }
}
