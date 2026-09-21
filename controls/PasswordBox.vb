Imports System.Drawing.Drawing2D
Public Class PasswordBox
    Public Property EnteredPassword As String
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.DialogResult = DialogResult.Cancel
        For i As Double = 1.0 To 0 Step -0.1
            Opacity = i
            Threading.Thread.Sleep(15)
        Next
        Me.Close()
    End Sub
    Private Sub btnClose_MouseEnter(sender As Object, e As EventArgs) Handles btnClose.MouseEnter
        btnClose.BackColor = Color.Red
    End Sub
    Private Sub btnClose_MouseLeave(sender As Object, e As EventArgs) Handles btnClose.MouseLeave
        btnClose.BackColor = Color.Transparent
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim radius As Integer = 50
        Dim path As New GraphicsPath()
        path.AddArc(0, 0, radius, radius, 180, 90)
        path.AddArc(Me.Width - radius, 0, radius, radius, 270, 90)
        path.AddArc(Me.Width - radius, Me.Height - radius, radius, radius, 0, 90)
        path.AddArc(0, Me.Height - radius, radius, radius, 90, 90)
        path.CloseFigure()
        Me.Region = New Region(path)
        MyBase.OnPaint(e)
    End Sub
    Private Sub PasswordBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler btnSubmit.Paint, AddressOf Manager.curvyBorders
    End Sub
    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        EnteredPassword = txbInput.Text
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
    Private Sub PasswordBox_Paint(sender As Object, e As PaintEventArgs) Handles MyBase.Paint
        Dim rect As New Rectangle(0, 0, Me.Width, Me.Height)
        Dim brush As New LinearGradientBrush(rect, Color.DarkMagenta, Color.DarkBlue, LinearGradientMode.Horizontal)
        e.Graphics.FillRectangle(brush, rect)
    End Sub
    Private Sub btnSubmit_MouseEnter(sender As Object, e As EventArgs) Handles btnSubmit.MouseEnter
        btnSubmit.BackColor = Color.AliceBlue
    End Sub
    Private Sub btnSubmit_MouseLeave(sender As Object, e As EventArgs) Handles btnSubmit.MouseLeave
        btnSubmit.BackColor = Color.Transparent
    End Sub
End Class