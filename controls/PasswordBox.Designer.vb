<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PasswordBox
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
        lbl = New Label()
        txbInput = New TextBox()
        btnSubmit = New PictureBox()
        btnClose = New Button()
        CType(btnSubmit, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' lbl
        ' 
        lbl.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        lbl.BackColor = Color.Transparent
        lbl.Font = New Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lbl.ForeColor = Color.AliceBlue
        lbl.Location = New Point(2, 66)
        lbl.Margin = New Padding(2, 0, 2, 0)
        lbl.Name = "lbl"
        lbl.RightToLeft = RightToLeft.No
        lbl.Size = New Size(566, 36)
        lbl.TabIndex = 2
        lbl.Text = "قـُم بإدخال كلمة المرور الخاصة بك"
        lbl.TextAlign = ContentAlignment.TopCenter
        ' 
        ' txbInput
        ' 
        txbInput.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        txbInput.BackColor = Color.AliceBlue
        txbInput.BorderStyle = BorderStyle.FixedSingle
        txbInput.Font = New Font("Book Antiqua", 14F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txbInput.Location = New Point(11, 115)
        txbInput.Margin = New Padding(2)
        txbInput.Name = "txbInput"
        txbInput.PasswordChar = "*"c
        txbInput.Size = New Size(550, 42)
        txbInput.TabIndex = 9
        txbInput.TextAlign = HorizontalAlignment.Center
        ' 
        ' btnSubmit
        ' 
        btnSubmit.BackColor = Color.Transparent
        btnSubmit.Image = My.Resources.Resources.login
        btnSubmit.Location = New Point(11, 191)
        btnSubmit.Margin = New Padding(2)
        btnSubmit.Name = "btnSubmit"
        btnSubmit.Size = New Size(550, 90)
        btnSubmit.SizeMode = PictureBoxSizeMode.Zoom
        btnSubmit.TabIndex = 10
        btnSubmit.TabStop = False
        ' 
        ' btnClose
        ' 
        btnClose.AutoSize = True
        btnClose.BackColor = Color.Transparent
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Image = My.Resources.Resources.close
        btnClose.Location = New Point(0, -1)
        btnClose.Margin = New Padding(0)
        btnClose.Name = "btnClose"
        btnClose.Size = New Size(82, 48)
        btnClose.TabIndex = 18
        btnClose.TextAlign = ContentAlignment.TopCenter
        btnClose.UseVisualStyleBackColor = False
        ' 
        ' PasswordBox
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Indigo
        ClientSize = New Size(572, 308)
        ControlBox = False
        Controls.Add(btnClose)
        Controls.Add(btnSubmit)
        Controls.Add(txbInput)
        Controls.Add(lbl)
        FormBorderStyle = FormBorderStyle.None
        Margin = New Padding(2)
        Name = "PasswordBox"
        Text = "PasswordBox"
        CType(btnSubmit, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lbl As Label
    Friend WithEvents txbInput As TextBox
    Friend WithEvents btnSubmit As PictureBox
    Friend WithEvents btnClose As Button
End Class
