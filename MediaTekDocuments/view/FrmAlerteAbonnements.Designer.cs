
namespace MediaTekDocuments.view
{
    partial class FrmAlerteAbonnements
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gBAlerte = new System.Windows.Forms.GroupBox();
            this.btnAlerteOk = new System.Windows.Forms.Button();
            this.dgvAlerte = new System.Windows.Forms.DataGridView();
            this.revue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateFinAbonnement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gBAlerte.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlerte)).BeginInit();
            this.SuspendLayout();
            // 
            // gBAlerte
            // 
            this.gBAlerte.Controls.Add(this.btnAlerteOk);
            this.gBAlerte.Controls.Add(this.dgvAlerte);
            this.gBAlerte.Location = new System.Drawing.Point(12, 12);
            this.gBAlerte.Name = "gBAlerte";
            this.gBAlerte.Size = new System.Drawing.Size(507, 206);
            this.gBAlerte.TabIndex = 17;
            this.gBAlerte.TabStop = false;
            this.gBAlerte.Text = " Abonnements expirant sous 30 jours";
            // 
            // btnAlerteOk
            // 
            this.btnAlerteOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlerteOk.Location = new System.Drawing.Point(411, 171);
            this.btnAlerteOk.Name = "btnAlerteOk";
            this.btnAlerteOk.Size = new System.Drawing.Size(75, 23);
            this.btnAlerteOk.TabIndex = 18;
            this.btnAlerteOk.Text = "OK";
            this.btnAlerteOk.UseVisualStyleBackColor = true;
            this.btnAlerteOk.Click += new System.EventHandler(this.btnAlerteOk_Click);
            // 
            // dgvAlerte
            // 
            this.dgvAlerte.AllowUserToAddRows = false;
            this.dgvAlerte.AllowUserToDeleteRows = false;
            this.dgvAlerte.AllowUserToResizeColumns = false;
            this.dgvAlerte.AllowUserToResizeRows = false;
            this.dgvAlerte.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAlerte.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAlerte.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAlerte.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.revue,
            this.DateFinAbonnement});
            this.dgvAlerte.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvAlerte.Location = new System.Drawing.Point(6, 33);
            this.dgvAlerte.MultiSelect = false;
            this.dgvAlerte.Name = "dgvAlerte";
            this.dgvAlerte.ReadOnly = true;
            this.dgvAlerte.RowHeadersVisible = false;
            this.dgvAlerte.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAlerte.Size = new System.Drawing.Size(480, 132);
            this.dgvAlerte.TabIndex = 17;
            // 
            // revue
            // 
            this.revue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.revue.DataPropertyName = "TitreRevue";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.revue.DefaultCellStyle = dataGridViewCellStyle2;
            this.revue.FillWeight = 65F;
            this.revue.HeaderText = "Revue";
            this.revue.Name = "revue";
            this.revue.ReadOnly = true;
            // 
            // DateFinAbonnement
            // 
            this.DateFinAbonnement.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DateFinAbonnement.DataPropertyName = "DateFinAbonnement";
            dataGridViewCellStyle3.Format = "d";
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DateFinAbonnement.DefaultCellStyle = dataGridViewCellStyle3;
            this.DateFinAbonnement.FillWeight = 35F;
            this.DateFinAbonnement.HeaderText = "Fin d\'abonnement";
            this.DateFinAbonnement.Name = "DateFinAbonnement";
            this.DateFinAbonnement.ReadOnly = true;
            // 
            // FrmAlerteAbonnements
            // 
            this.AcceptButton = this.btnAlerteOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 231);
            this.Controls.Add(this.gBAlerte);
            this.Name = "FrmAlerteAbonnements";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Abonnements à renouveler";
            this.gBAlerte.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlerte)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gBAlerte;
        private System.Windows.Forms.Button btnAlerteOk;
        private System.Windows.Forms.DataGridView dgvAlerte;
        private System.Windows.Forms.DataGridViewTextBoxColumn revue;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateFinAbonnement;
    }
}