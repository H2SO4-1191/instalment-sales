<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PayBox
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        btnClose = New Button()
        btnSubmit = New PictureBox()
        txbInput = New NumericUpDown()
        lbl = New Label()
        CType(btnSubmit, ComponentModel.ISupportInitialize).BeginInit()
        CType(txbInput, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnClose
        ' 
        btnClose.AutoSize = True
        btnClose.BackColor = Color.Transparent
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Image = My.Resources.Resources.close
        btnClose.Location = New Point(-7, -2)
        btnClose.Margin = New Padding(0)
        btnClose.Name = "btnClose"
        btnClose.Size = New Size(82, 48)
        btnClose.TabIndex = 21
        btnClose.TextAlign = ContentAlignment.TopCenter
        btnClose.UseVisualStyleBackColor = False
        ' 
        ' btnSubmit
        ' 
        btnSubmit.BackColor = Color.Transparent
        btnSubmit.Image = My.Resources.Resources.login
        btnSubmit.Location = New Point(11, 207)
        btnSubmit.Margin = New Padding(2)
        btnSubmit.Name = "btnSubmit"
        btnSubmit.Size = New Size(550, 90)
        btnSubmit.SizeMode = PictureBoxSizeMode.Zoom
        btnSubmit.TabIndex = 20
        btnSubmit.TabStop = False
        ' 
        ' txbInput
        ' 
        txbInput.BackColor = Color.Ivory
        txbInput.BorderStyle = BorderStyle.FixedSingle
        txbInput.Font = New Font("Times New Roman", 16F, FontStyle.Bold)
        txbInput.Increment = New Decimal(New Integer() {5000, 0, 0, 0})
        txbInput.Location = New Point(11, 134)
        txbInput.Margin = New Padding(2)
        txbInput.Maximum = New Decimal(New Integer() {-1981284353, -1966660860, 0, 0})
        txbInput.Minimum = New Decimal(New Integer() {5000, 0, 0, 0})
        txbInput.Name = "txbInput"
        txbInput.RightToLeft = RightToLeft.Yes
        txbInput.Size = New Size(550, 44)
        txbInput.TabIndex = 22
        txbInput.TextAlign = HorizontalAlignment.Center
        txbInput.ThousandsSeparator = True
        txbInput.Value = New Decimal(New Integer() {5000, 0, 0, 0})
        ' 
        ' lbl
        ' 
        lbl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lbl.BackColor = Color.Transparent
        lbl.Font = New Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lbl.ForeColor = Color.AliceBlue
        lbl.Location = New Point(2, 74)
        lbl.Margin = New Padding(2, 0, 2, 0)
        lbl.Name = "lbl"
        lbl.RightToLeft = RightToLeft.No
        lbl.Size = New Size(566, 36)
        lbl.TabIndex = 23
        lbl.Text = "أدخِل مبلغ القسط"
        lbl.TextAlign = ContentAlignment.TopCenter
        ' 
        ' PayBox
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Indigo
        ClientSize = New Size(572, 308)
        ControlBox = False
        Controls.Add(lbl)
        Controls.Add(txbInput)
        Controls.Add(btnClose)
        Controls.Add(btnSubmit)
        FormBorderStyle = FormBorderStyle.None
        Name = "PayBox"
        ShowIcon = False
        StartPosition = FormStartPosition.CenterParent
        CType(btnSubmit, ComponentModel.ISupportInitialize).EndInit()
        CType(txbInput, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnClose As Button
    Friend WithEvents btnSubmit As PictureBox
    Friend WithEvents txbInput As NumericUpDown
    Friend WithEvents lbl As Label
End Class
