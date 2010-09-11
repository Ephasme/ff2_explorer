namespace FFR2Explorer {
    partial class FFR2MainForm {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Intérieures");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Extérieures");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Maître du Jeu");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Système");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Hors charte");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Aires", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11});
            this.labTemp0Path = new System.Windows.Forms.Label();
            this.txtbTemp0Path = new System.Windows.Forms.TextBox();
            this.butDefTemp0Path = new System.Windows.Forms.Button();
            this.pgbMain = new System.Windows.Forms.ProgressBar();
            this.butRefresh = new System.Windows.Forms.Button();
            this.lblRessList = new System.Windows.Forms.Label();
            this.trvRessList = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // labTemp0Path
            // 
            this.labTemp0Path.AutoSize = true;
            this.labTemp0Path.Location = new System.Drawing.Point(9, 531);
            this.labTemp0Path.Name = "labTemp0Path";
            this.labTemp0Path.Size = new System.Drawing.Size(98, 13);
            this.labTemp0Path.TabIndex = 0;
            this.labTemp0Path.Text = "Répertoire Temp0 :";
            // 
            // txtbTemp0Path
            // 
            this.txtbTemp0Path.Location = new System.Drawing.Point(107, 528);
            this.txtbTemp0Path.Name = "txtbTemp0Path";
            this.txtbTemp0Path.Size = new System.Drawing.Size(219, 20);
            this.txtbTemp0Path.TabIndex = 1;
            // 
            // butDefTemp0Path
            // 
            this.butDefTemp0Path.FlatAppearance.BorderSize = 0;
            this.butDefTemp0Path.Location = new System.Drawing.Point(388, 528);
            this.butDefTemp0Path.Margin = new System.Windows.Forms.Padding(0);
            this.butDefTemp0Path.Name = "butDefTemp0Path";
            this.butDefTemp0Path.Size = new System.Drawing.Size(46, 19);
            this.butDefTemp0Path.TabIndex = 2;
            this.butDefTemp0Path.Text = "Définir";
            this.butDefTemp0Path.UseVisualStyleBackColor = true;
            // 
            // pgbMain
            // 
            this.pgbMain.Location = new System.Drawing.Point(12, 499);
            this.pgbMain.Name = "pgbMain";
            this.pgbMain.Size = new System.Drawing.Size(422, 23);
            this.pgbMain.TabIndex = 3;
            // 
            // butRefresh
            // 
            this.butRefresh.Location = new System.Drawing.Point(332, 528);
            this.butRefresh.Name = "butRefresh";
            this.butRefresh.Size = new System.Drawing.Size(53, 20);
            this.butRefresh.TabIndex = 4;
            this.butRefresh.Text = "Refresh";
            this.butRefresh.UseVisualStyleBackColor = true;
            // 
            // lblRessList
            // 
            this.lblRessList.AutoSize = true;
            this.lblRessList.Location = new System.Drawing.Point(12, 32);
            this.lblRessList.Name = "lblRessList";
            this.lblRessList.Size = new System.Drawing.Size(114, 13);
            this.lblRessList.TabIndex = 6;
            this.lblRessList.Text = "Liste des Ressources :";
            // 
            // trvRessList
            // 
            this.trvRessList.Location = new System.Drawing.Point(5, 48);
            this.trvRessList.Name = "trvRessList";
            treeNode7.Name = "ndIntAreas";
            treeNode7.Text = "Intérieures";
            treeNode8.Name = "ndExtAreas";
            treeNode8.Text = "Extérieures";
            treeNode9.Name = "ndDMAreas";
            treeNode9.Text = "Maître du Jeu";
            treeNode10.Name = "ndSysArea";
            treeNode10.Text = "Système";
            treeNode11.Name = "Noeud3";
            treeNode11.Text = "Hors charte";
            treeNode12.Name = "ndAreas";
            treeNode12.Text = "Aires";
            this.trvRessList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode12});
            this.trvRessList.Size = new System.Drawing.Size(198, 445);
            this.trvRessList.TabIndex = 7;
            // 
            // FFR2MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 553);
            this.Controls.Add(this.trvRessList);
            this.Controls.Add(this.lblRessList);
            this.Controls.Add(this.butRefresh);
            this.Controls.Add(this.pgbMain);
            this.Controls.Add(this.butDefTemp0Path);
            this.Controls.Add(this.txtbTemp0Path);
            this.Controls.Add(this.labTemp0Path);
            this.Name = "FFR2MainForm";
            this.Text = "FFR2Explorer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labTemp0Path;
        private System.Windows.Forms.TextBox txtbTemp0Path;
        private System.Windows.Forms.Button butDefTemp0Path;
        private System.Windows.Forms.ProgressBar pgbMain;
        private System.Windows.Forms.Button butRefresh;
        private System.Windows.Forms.Label lblRessList;
        private System.Windows.Forms.TreeView trvRessList;
    }
}

