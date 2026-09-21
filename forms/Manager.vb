Imports System.Data.OleDb
Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Security.Cryptography
Imports System.Drawing.Printing
Imports System.Globalization
Imports System.Threading
Public Class Manager
    Dim db As OleDbConnection
    Dim navBar() As Control
    Dim navBarSelected As Control
    Dim productToBuy As Product
    Dim customerToBuy As Customer
    Dim saleToHappen As Sale
    Dim adminInControl As String
    Dim adminInControPassword As String
    Dim lblDayPrev As String
    Dim lblMonthPrev As String
    Dim lblDeptPrev As String
    Dim expanding As Boolean = False
    Dim collapsing As Boolean = False
    Dim inSales As Boolean = False
    Dim inStore As Boolean = False
    Dim maxHeight As Integer = 100
    Dim baseTop As Integer
    Dim customerIDSearch As String
    Dim productPay As String
    Dim remainderPay As Decimal
    Dim d As PayBox
    Dim printDoc As PrintDocument
    Dim printDoc2 As PrintDocument
    Dim selectedRow As DataGridViewRow
    Dim ttRemain = 0
    Dim all = False
    Dim last = False
    Dim currentPrintIndex As Integer = 0
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        setupEssentials()
    End Sub
    Private Sub setupEssentials()
        Thread.CurrentThread.CurrentCulture = New CultureInfo("en-GB")
        Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-GB")
        btnClose.FlatAppearance.BorderSize = 0
        btnMinimize.FlatAppearance.BorderSize = 0
        navBar = {navBtnHome, navBtnBuy, navBtnManage, navBtnAdd, navBtnCreateAccount}
        db = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Application.StartupPath & "\MyDatabase.accdb;")
        Dim btns() As Control = {btnLogin, btnBuyCash, btnBuyLater, btnInsBuy, btnBuyBack, btnInsBack, btnAdd, btnLogout, btnClose, btnMinimize, btnCreateAdmin}
        For Each btn As Control In btns
            AddHandler btn.Paint, AddressOf curvyBorders
        Next
        printDoc = New PrintDocument()
        printDoc2 = New PrintDocument()
        printDoc.PrinterSettings.PrinterName = "POS-80-Series"
        printDoc2.PrinterSettings.PrinterName = "POS-80-Series"
        AddHandler printDoc.PrintPage, AddressOf Me.PrintPageHandler
        AddHandler printDoc2.PrintPage, AddressOf Me.PrintPageHandler2
    End Sub
    <DllImport("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function
    <DllImport("user32.dll")>
    Public Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As Integer, lParam As Integer) As Boolean
    End Function
    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTCAPTION As Integer = &H2
    Private Sub Form1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        If e.Button = MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0)
        End If
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
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        For i As Double = 1.0 To 0 Step -0.1
            Opacity = i
            Threading.Thread.Sleep(15)
        Next
        Close()
    End Sub
    Private Sub btnMinimize_Click(sender As Object, e As EventArgs) Handles btnMinimize.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub
    Private Sub btnClose_MouseEnter(sender As Object, e As EventArgs) Handles btnClose.MouseEnter
        btnClose.BackColor = Color.Red
    End Sub
    Private Sub btnClose_MouseLeave(sender As Object, e As EventArgs) Handles btnClose.MouseLeave
        btnClose.BackColor = Color.Transparent
    End Sub
    Private Sub btnMin_MouseEnter(sender As Object, e As EventArgs) Handles btnMinimize.MouseEnter
        btnMinimize.BackColor = Color.SkyBlue
    End Sub
    Private Sub btnMin_MouseLeave(sender As Object, e As EventArgs) Handles btnMinimize.MouseLeave
        btnMinimize.BackColor = Color.Transparent
    End Sub
    Private Sub nav_Paint(sender As Object, e As PaintEventArgs) Handles nav.Paint
        Dim rect As New Rectangle(0, 0, nav.Width, nav.Height)
        Dim brush As New LinearGradientBrush(rect, Color.DarkBlue, Color.DarkMagenta, LinearGradientMode.Horizontal)
        e.Graphics.FillRectangle(brush, rect)
    End Sub
    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        pnlLogin.Enabled = False
        Dim errorTitle As String = "فشل تسجيل الدخول"
        Dim username As String = boxAdminUsername.Text
        Dim password As String = boxAdminPassword.Text
        If (String.IsNullOrEmpty(username)) Then
            MsgBox("عليك إدخال المعرف الخاص بك أولاً", MsgBoxStyle.Critical, errorTitle)
        ElseIf (String.IsNullOrEmpty(password)) Then
            MsgBox("عليك إدخال كلمة المرور الخاصة بك أولاً", MsgBoxStyle.Critical, errorTitle)
        Else
            If (validateLogin(username, password)) Then
                adminInControl = username
                adminInControPassword = hashPassword(password)
                Timer1.Interval = 10
                Timer1.Start()
                setupNavBar(True)
            Else
                MsgBox("هناك خطبٌ ما في المعلومات التي أدخلتها , أعِد التحقق من صحّتها", MsgBoxStyle.Critical, errorTitle)
            End If
        End If
        pnlLogin.Enabled = True
    End Sub
    Private Function validateLogin(ID As String, password As String) As Boolean
        db.Open()
        Using cmd As New OleDbCommand("SELECT COUNT(*) FROM Admins WHERE ID = ? AND passwordHash = ?", db)
            cmd.Parameters.AddWithValue("?", ID)
            cmd.Parameters.AddWithValue("?", hashPassword(password))
            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            db.Close()
            Return count > 0
        End Using
    End Function
    Public Sub curvyBorders(sender As Object, e As PaintEventArgs)
        Dim control = DirectCast(sender, Control)
        Dim radius = 50
        Dim path As New GraphicsPath
        path.AddArc(0, 0, radius, radius, 180, 90)
        path.AddArc(control.Width - radius, 0, radius, radius, 270, 90)
        path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90)
        path.AddArc(0, control.Height - radius, radius, radius, 90, 90)
        path.CloseFigure()
        control.Region = New Region(path)
    End Sub
    Private Sub btnLogin_MouseEnter(sender As Object, e As EventArgs) Handles btnLogin.MouseEnter
        btnLogin.BackColor = Color.DarkMagenta
    End Sub
    Private Sub btnLogin_MouseLeave(sender As Object, e As EventArgs) Handles btnLogin.MouseLeave
        btnLogin.BackColor = Color.Transparent
    End Sub
    Private Sub setupNavBar(state As Boolean)
        'true = in, false = out'
        boxAdminUsername.Text = ""
        boxAdminPassword.Text = ""
        pnlLoginPic1.Visible = Not (state)
        pnlLoginPic2.Visible = Not (state)
        pnlLoginLbl1.Visible = Not (state)
        pnlLoginLbl2.Visible = Not (state)
        navAdminUsername.Visible = state
        For Each control In navBar
            control.Visible = state
        Next
        btnLogout.Visible = state
        If state Then
            navAdminUsername.Text = "الإداري: " + adminInControl.Substring(0, adminInControl.IndexOf("@"))
            navBarSelected = navBtnHome
            navBtnHome.ForeColor = Color.Gold
            navBtnHome.Font = New Font("Times New Roman", 16, FontStyle.Underline)
            For Each control In navBar
                AddHandler control.MouseEnter, AddressOf nav_MouseEnter
                AddHandler control.MouseLeave, AddressOf nav_MouseLeave
                AddHandler control.Click, AddressOf setSelected
            Next
        Else
            navAdminUsername.Text = ""
        End If
        nav.Refresh()
    End Sub
    Private Sub nav_MouseEnter(sender As Object, e As EventArgs)
        Dim control As Control = CType(sender, Control)
        If Not (control.ForeColor = Color.Gold) Then
            control.BackColor = Color.Indigo
            control.ForeColor = Color.Cyan
            control.Font = New Font("Times New Roman", 22)
        End If
    End Sub
    Private Sub nav_MouseLeave(sender As Object, e As EventArgs)
        Dim control As Control = CType(sender, Control)
        If Not (control.ForeColor = Color.Gold) Then
            control.BackColor = Color.Transparent
            control.ForeColor = Color.White
            control.Font = New Font("Times New Roman", 16)
        End If
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If pnlLogin.Left > -1250 Then
            pnlLogin.Left -= 25
        Else
            pnlLogin.Visible = False
            Timer1.Stop()
        End If
    End Sub
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        If pnlLogin.Left < 0 Then
            pnlLogin.Left += 25
        Else
            Timer2.Stop()
        End If
    End Sub
    Private Sub setSelected(sender As Object, e As EventArgs)
        For Each control As Control In pnlCreateAdmin.Controls
            control.Visible = False
        Next
        Dim navBtn As Control = CType(sender, Control)
        navBarSelected.ForeColor = Color.White
        navBarSelected.Font = New Font("Times New Roman", 16, FontStyle.Regular)
        navBtn.ForeColor = Color.Gold
        navBtn.Font = New Font("Times New Roman", 16, FontStyle.Underline)
        navBtn.BackColor = Color.Transparent
        Select Case navBtn.Text
            Case "الأساسية"
                'Not implemented yet.'
            Case "شِراء"
                setupBuy()
            Case "الإدارة"
                setupManagement()
            Case "إضافة عناصر"
                setupAdd()
            Case "إنشاء حساب"
                setupCreateAdmin()
        End Select
        navBarSelected = navBtn
    End Sub
    Private Sub setupBuy()
        Dim cmd As New OleDbCommand("SELECT * FROM Products", db)
        Dim adapter As New OleDbDataAdapter(cmd)
        Dim productsTable As New DataTable()
        db.Open()
        adapter.Fill(productsTable)
        db.Close()
        pnlProducts.Controls.Clear()
        For Each row As DataRow In productsTable.Rows
            Dim product As New Product(
                row("ID").ToString(),
                row("device").ToString(),
                row("manufacturer").ToString(),
                row("model").ToString(),
                row("madeIn").ToString(),
                row("proYear"),
                CDec(row("price")),
                CInt(row("quantity"))
            )
            AddProduct(product)
        Next
        pnlProducts.BringToFront()
    End Sub
    Private Sub setupAdd()
        AddHandler btnAdd.MouseEnter, AddressOf nav_MouseEnter
        AddHandler btnAdd.MouseLeave, AddressOf nav_MouseLeave
        AddHandler txbAddYear.KeyPress, AddressOf onlyDigits
        pnlAdd.BringToFront()
    End Sub
    Private Sub setupCreateAdmin()
        pnlCreateAdmin.BringToFront()
        Dim passForm As New PasswordBox()
        passForm.StartPosition = FormStartPosition.Manual
        Dim parentCenterX = Me.Left + (Me.Width \ 2)
        Dim parentCenterY = Me.Top + (Me.Height \ 2)
        passForm.Location = New Point(parentCenterX - (passForm.Width \ 2) - 50, parentCenterY - (passForm.Height \ 2))
        If passForm.ShowDialog() = DialogResult.OK Then
            If hashPassword(passForm.EnteredPassword) = adminInControPassword Then
                For Each control As Control In pnlCreateAdmin.Controls
                    control.Visible = True
                Next
                AddHandler btnCreateAdmin.MouseEnter, AddressOf nav_MouseEnter
                AddHandler btnCreateAdmin.MouseLeave, AddressOf nav_MouseLeave
                AddHandler txbCreatePhone.KeyPress, AddressOf onlyDigits
            Else
                MsgBox("كلمة المرور خاطئة", MsgBoxStyle.Critical)
            End If
        End If
    End Sub
    Private Sub setupManagement()
        AddHandler btnManageStore.Paint, AddressOf curvyBorders
        AddHandler btnManageStore.MouseEnter, AddressOf nav_MouseEnter
        AddHandler btnManageStore.MouseLeave, AddressOf nav_MouseLeave
        AddHandler btnManageSale.Paint, AddressOf curvyBorders
        AddHandler btnManageSale.MouseEnter, AddressOf nav_MouseEnter
        AddHandler btnManageSale.MouseLeave, AddressOf nav_MouseLeave
        AddHandler btnSearchManagement.Paint, AddressOf curvyBorders
        AddHandler btnSearchManagement.MouseEnter, AddressOf nav_MouseEnter
        AddHandler btnSearchManagement.MouseLeave, AddressOf nav_MouseLeave
        AddHandler btnBackManagement.Paint, AddressOf curvyBorders
        AddHandler btnBackManagement.MouseEnter, AddressOf nav_MouseEnter
        AddHandler btnBackManagement.MouseLeave, AddressOf nav_MouseLeave
        pnlManagement.BringToFront()
    End Sub
    Private Sub AddProduct(product As Product)
        Dim productPanel As Panel
        If product.Quantity > 0 Then
            productPanel = New Panel With {
                .Size = New Size(375, 420),
                .BackColor = Color.Indigo
            }
        Else
            productPanel = New Panel With {
                .Size = New Size(375, 420),
                .BackColor = Color.FromArgb(64, 64, 64)
            }
        End If
        AddHandler productPanel.Paint, AddressOf curvyBorders
        Dim lblDevice As New Label With {
        .Text = "الجهاز: " & product.Device,
        .Width = 375,
        .Height = 45,
        .Location = New Point(5, 10),
        .ForeColor = Color.Cyan,
        .RightToLeft = RightToLeft.Yes,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Font = New Font("Times New Roman", 14, FontStyle.Regular)
    }
        Dim lblManufacturer As New Label With {
        .Text = "الشركة المُصنّعة: " & product.Manufacturer,
        .Width = 375,
        .Height = 45,
        .Location = New Point(5, lblDevice.Bottom + 10),
        .ForeColor = Color.Cyan,
        .RightToLeft = RightToLeft.Yes,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Font = New Font("Times New Roman", 14, FontStyle.Regular)
    }
        Dim lblModel As New Label With {
        .Text = "الموديل: " & product.Model,
        .Width = 375,
        .Height = 45,
        .Location = New Point(5, lblManufacturer.Bottom + 10),
        .ForeColor = Color.Cyan,
        .RightToLeft = RightToLeft.Yes,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Font = New Font("Times New Roman", 14, FontStyle.Regular)
    }
        Dim lblMadeIn As New Label With {
        .Text = "صُنع في: " & product.MadeIn,
        .Width = 375,
        .Height = 45,
        .Location = New Point(5, lblModel.Bottom + 10),
        .ForeColor = Color.Cyan,
        .RightToLeft = RightToLeft.Yes,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Font = New Font("Times New Roman", 14, FontStyle.Regular)
    }
        Dim lblYear As New Label With {
        .Text = "سنة الإنتاج: " & product.Year,
        .Width = 375,
        .Height = 45,
        .Location = New Point(5, lblMadeIn.Bottom + 10),
        .ForeColor = Color.Cyan,
        .RightToLeft = RightToLeft.Yes,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Font = New Font("Times New Roman", 14, FontStyle.Regular)
    }
        Dim lblPrice As New Label With {
        .Text = "السعر: " & product.Price.ToString("N0") & " د.ع",
        .Width = 375,
        .Height = 45,
        .Location = New Point(5, lblYear.Bottom + 10),
        .ForeColor = Color.Gold,
        .RightToLeft = RightToLeft.Yes,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Font = New Font("Times New Roman", 14, FontStyle.Regular)
    }
        Dim btnProductBuy As New Button With {
        .Text = "شِراء",
        .Name = product.ID,
        .Width = 355,
        .Height = 60,
        .Location = New Point(10, lblPrice.Bottom + 15),
        .ForeColor = Color.DarkMagenta,
        .BackColor = Color.AliceBlue,
        .RightToLeft = RightToLeft.Yes,
        .Font = New Font("New Times Roman", 14, FontStyle.Bold),
        .TextAlign = ContentAlignment.MiddleCenter
    }
        productPanel.Controls.Add(lblDevice)
        productPanel.Controls.Add(lblManufacturer)
        productPanel.Controls.Add(lblModel)
        productPanel.Controls.Add(lblMadeIn)
        productPanel.Controls.Add(lblYear)
        productPanel.Controls.Add(lblPrice)
        productPanel.Controls.Add(btnProductBuy)
        AddHandler btnProductBuy.Click, AddressOf setBuyDetails
        AddHandler btnProductBuy.MouseEnter, AddressOf btnBuy_MouseEnter
        AddHandler btnProductBuy.MouseLeave, AddressOf btnBuy_MouseLeave
        pnlProducts.Controls.Add(productPanel)
    End Sub
    Private Sub setBuyDetails(sender As Object, e As EventArgs)
        Dim product As Control = CType(sender, Control)
        Dim cmd As New OleDbCommand("SELECT * FROM Products WHERE ID = ?", db)
        cmd.Parameters.AddWithValue("?", product.Name)
        Dim adapter As New OleDbDataAdapter(cmd)
        Dim productTable As New DataTable()
        db.Open()
        adapter.Fill(productTable)
        db.Close()
        AddHandler btnBuyCash.MouseEnter, AddressOf buy_MouseEnter
        AddHandler btnBuyCash.MouseLeave, AddressOf buy_MouseLeave
        AddHandler btnBuyLater.MouseEnter, AddressOf buy_MouseEnter
        AddHandler btnBuyLater.MouseLeave, AddressOf buy_MouseLeave
        AddHandler btnBuyBack.MouseEnter, AddressOf buy_MouseEnter
        AddHandler btnBuyBack.MouseLeave, AddressOf buy_MouseLeave
        AddHandler btnInsBuy.MouseEnter, AddressOf buy_MouseEnter
        AddHandler btnInsBuy.MouseLeave, AddressOf buy_MouseLeave
        AddHandler btnInsBack.MouseEnter, AddressOf buy_MouseEnter
        AddHandler btnInsBack.MouseLeave, AddressOf buy_MouseLeave
        AddHandler txbCustomerPhone.KeyPress, AddressOf onlyDigits
        Dim row As DataRow = productTable.Rows(0)
        productToBuy = New Product(
            row("ID").ToString,
            row("device").ToString,
            row("manufacturer").ToString,
            row("model").ToString,
            row("madeIn").ToString,
            row("proYear").ToString,
            row("price"),
            row("quantity")
        )
        lblDevice.Text = "الجهاز: " & productToBuy.Device
        lblManufacturer.Text = "الشركة المصنّعة: " & productToBuy.Manufacturer
        lblModel.Text = "الموديل: " & productToBuy.Model
        lblMadeIn.Text = "صُنع في: " & productToBuy.MadeIn
        lblProYear.Text = "سنة الإنتاج: " & productToBuy.Year
        lblInsDevice.Text = "الجهاز: " & productToBuy.Device
        lblInsManufacturer.Text = "الشركة المصنّعة: " & productToBuy.Manufacturer
        lblInsModel.Text = "الموديل: " & productToBuy.Model
        lblInsMadeIn.Text = "صُنع في: " & productToBuy.MadeIn
        lblInsProYear.Text = "سنة الإنتاج: " & productToBuy.Year
        lblState.Text = "المتوفر: " & productToBuy.Quantity
        lblInsState.Text = "المتوفر: " & productToBuy.Quantity
        txbProductPrice.Text = productToBuy.Price
        pnlBuyDetails.BringToFront()
    End Sub
    Private Sub buy_MouseEnter(sender As Object, e As EventArgs)
        Dim control As Control = CType(sender, Control)
        control.BackColor = Color.Indigo
    End Sub
    Private Sub buy_MouseLeave(sender As Object, e As EventArgs)
        Dim control As Control = CType(sender, Control)
        control.BackColor = Color.Transparent
    End Sub
    Private Sub btnBuy_MouseEnter(sender As Object, e As EventArgs)
        Dim control As Control = CType(sender, Control)
        control.BackColor = Color.DarkMagenta
        control.ForeColor = Color.AliceBlue
    End Sub
    Private Sub btnBuy_MouseLeave(sender As Object, e As EventArgs)
        Dim control As Control = CType(sender, Control)
        control.BackColor = Color.AliceBlue
        control.ForeColor = Color.DarkMagenta
    End Sub
    Private Sub btnBuyCash_Click(sender As Object, e As EventArgs) Handles btnBuyCash.Click
        If productToBuy.Quantity > 0 Then
            If checkInput(pnlBuyDetails) Then
                Dim result = MessageBox.Show("هل أنت متأكد من الرغبة بالتقدم؟", "تأكيد الشراء نقداً", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    setCustomerToBuy()
                    db.Open()
                    validateAndAddCustomer()
                    productToBuy.Price = txbProductPrice.Value
                    setSaleToHappen()
                    printDoc2.Print()
                    addCashBuyToDB()
                    decrementProductToBuyQuantity()
                    db.Close()
                    pnlProducts.BringToFront()
                    resetBuyingPanels()
                End If
            Else
                MsgBox("الرجاء التأكد من ملئ الحقول المطلوبة بدقّة", MsgBoxStyle.Critical, "فشل عملية الشِراء")
            End If
        Else
            MsgBox("الجهاز غير متوفّر", MsgBoxStyle.Critical, "فشل عملية الشِراء")
        End If
    End Sub
    Private Sub btnBuyLater_Click(sender As Object, e As EventArgs) Handles btnBuyLater.Click
        If productToBuy.Quantity > 0 Then
            If checkInput(pnlBuyDetails) Then
                setCustomerToBuy()
                productToBuy.Price = txbProductPrice.Value
                setSaleToHappen()
                setInsDetails()
                lblInsBasePrice.Text = "السعر الأسمي (د.ع): " & productToBuy.Price.ToString("N0")
                lblInsRemaining.Text = "المبلغ المتبقّي (د.ع): " & saleToHappen.Remaining.ToString("N0")
                lblInsPriceAfter.Text = "السعر بالتقسيط (د.ع): " & saleToHappen.InstalmentPrice.ToString("N0")
                AddHandler txbInsFirst.ValueChanged, AddressOf txb_ValueChanged
                AddHandler txbInsloadValue.ValueChanged, AddressOf txb_ValueChanged
                AddHandler txbInsLoadForeach.ValueChanged, AddressOf txb_ValueChanged
                pnlInstalmentBuy.BringToFront()
            Else
                MsgBox("الرجاء التأكد من ملئ الحقول المطلوبة بدقّة", MsgBoxStyle.Critical, "فشل عملية الشِراء")
            End If
        Else
            MsgBox("الجهاز غير متوفّر", MsgBoxStyle.Critical, "فشل عملية الشِراء")
        End If
    End Sub
    Private Sub btnInsBuy_Click(sender As Object, e As EventArgs) Handles btnInsBuy.Click
        Dim result = MessageBox.Show("هل أنت متأكد من الرغبة بالتقدم؟", "تأكيد الشراء بالتقسيط", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            db.Open()
            validateAndAddCustomer()
            setInsDetails()
            printDoc2.Print()
            addInsBuyToDB()
            decrementProductToBuyQuantity()
            db.Close()
            pnlProducts.BringToFront()
            resetBuyingPanels()
        End If
    End Sub
    Private Sub validateAndAddCustomer()
        Using cmd As New OleDbCommand("SELECT COUNT(*) FROM Customers WHERE ID = ?", db)
            cmd.Parameters.AddWithValue("?", customerToBuy.ID)
            Dim count = Convert.ToInt32(cmd.ExecuteScalar)
            If count <= 0 Then
                Using cmdIns As New OleDbCommand("INSERT INTO Customers (ID, name, address, phoneNumber) VALUES (?, ?, ?, ?)", db)
                    cmdIns.Parameters.AddWithValue("?", customerToBuy.ID)
                    cmdIns.Parameters.AddWithValue("?", customerToBuy.Name)
                    cmdIns.Parameters.AddWithValue("?", customerToBuy.Address)
                    cmdIns.Parameters.AddWithValue("?", customerToBuy.Phone)
                    cmdIns.ExecuteNonQuery()
                    MsgBox("تمت إضافة زبون جديد الى قاعدة البيانات", MsgBoxStyle.Information, "أول عملية شراء لهذا الزبون")
                End Using
            End If
        End Using
    End Sub
    Private Sub decrementProductToBuyQuantity()
        Using cmd As New OleDbCommand("UPDATE Products SET quantity = ? WHERE ID = ?", db)
            cmd.Parameters.AddWithValue("?", productToBuy.Quantity - 1)
            cmd.Parameters.AddWithValue("?", productToBuy.ID)
            cmd.ExecuteNonQuery()
        End Using
    End Sub
    Private Sub resetBuyingPanels()
        For Each control As Control In pnlBuyDetails.Controls
            If TypeOf control Is TextBox Then
                control.Text = ""
            End If
        Next
        txbInsLoadForeach.Value = 100000
        txbInsloadValue.Value = 50000
        txbInsFirst.Value = 0
    End Sub
    Private Function checkInput(pnl As GroupBox) As Boolean
        For Each control As Control In pnl.Controls
            If TypeOf control Is TextBox Then
                If control.Text = "" Then
                    Return False
                End If
                If control Is txbCustomerUsername Or control Is txbCreateEmail Then
                    Return Regex.IsMatch(control.Text, "^[^@]+@[^@]+\.[^@]+$")
                End If
            End If
        Next
        Return True
    End Function
    Private Sub setCustomerToBuy()
        customerToBuy = New Customer(
                    txbCustomerUsername.Text,
                    txbCustomerName.Text,
                    txbCustomerAddress.Text,
                    txbCustomerPhone.Text
                    )
    End Sub
    Private Sub setSaleToHappen()
        saleToHappen = New Sale(
                customerToBuy.ID,
                productToBuy.ID,
                adminInControl,
                productToBuy.Price,
                Now.ToString("yyyy-MM-dd HH:mm")
                )
    End Sub
    Private Sub addCashBuyToDB()
        Using cmd As New OleDbCommand("INSERT INTO Sales (customerID, adminID, productID, price, purchaseDate) VALUES (?, ?, ?, ?, ?)", db)
            cmd.Parameters.AddWithValue("?", saleToHappen.CustomerID)
            cmd.Parameters.AddWithValue("?", saleToHappen.AdminID)
            cmd.Parameters.AddWithValue("?", saleToHappen.ProductID)
            cmd.Parameters.AddWithValue("?", saleToHappen.Price)
            cmd.Parameters.AddWithValue("?", saleToHappen.PurchaseDate)
            cmd.ExecuteNonQuery()
            MsgBox("تمّ تسجيل عملية الشِراء في قاعدة البيانات", MsgBoxStyle.Information, "نجاح")
        End Using
    End Sub
    Private Sub setInsDetails()
        saleToHappen.InstalmentPrice = (productToBuy.Price / txbInsLoadForeach.Value * txbInsloadValue.Value) + productToBuy.Price
        saleToHappen.FirstInstalment = txbInsFirst.Value
        saleToHappen.Remaining = saleToHappen.InstalmentPrice - saleToHappen.FirstInstalment
    End Sub
    Private Sub addInsBuyToDB()
        Using cmd As New OleDbCommand("INSERT INTO Sales (customerID, adminID, productID, price, purchaseDate, instalmentPrice, remaining, 1instalment) VALUES (?, ?, ?, ?, ?, ?, ?, ?)", db)
            cmd.Parameters.AddWithValue("?", saleToHappen.CustomerID)
            cmd.Parameters.AddWithValue("?", saleToHappen.AdminID)
            cmd.Parameters.AddWithValue("?", saleToHappen.ProductID)
            cmd.Parameters.AddWithValue("?", saleToHappen.Price)
            cmd.Parameters.AddWithValue("?", saleToHappen.PurchaseDate)
            cmd.Parameters.AddWithValue("?", saleToHappen.InstalmentPrice)
            cmd.Parameters.AddWithValue("?", saleToHappen.Remaining)
            cmd.Parameters.AddWithValue("?", saleToHappen.FirstInstalment)
            cmd.ExecuteNonQuery()
            MsgBox("تمّ تسجيل عملية الشِراء في قاعدة البيانات", MsgBoxStyle.Information, "نجاح")
        End Using
    End Sub
    Private Sub txb_ValueChanged(sender As Object, e As EventArgs)
        setInsDetails()
        lblInsRemaining.Text = "المبلغ المتبقّي (د.ع): " & saleToHappen.Remaining.ToString("N0")
        lblInsPriceAfter.Text = "السعر بالتقسيط (د.ع): " & saleToHappen.InstalmentPrice.ToString("N0")
    End Sub
    Private Sub btnBuyBack_Click(sender As Object, e As EventArgs) Handles btnBuyBack.Click
        pnlProducts.BringToFront()
        resetBuyingPanels()
    End Sub
    Private Sub btnInsBack_Click(sender As Object, e As EventArgs) Handles btnInsBack.Click
        pnlBuyDetails.BringToFront()
    End Sub
    Private Sub btnLogout_MouseEnter(sender As Object, e As EventArgs) Handles btnLogout.MouseEnter
        btnLogout.BackColor = Color.DarkBlue
    End Sub
    Private Sub btnLogout_MouseLeave(sender As Object, e As EventArgs) Handles btnLogout.MouseLeave
        btnLogout.BackColor = Color.Transparent
    End Sub
    Private Sub onlyDigits(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If checkInput(pnlAdd) Then
            Dim result = MessageBox.Show("هل أنت متأكد من الرغبة بالتقدم؟", "تأكيد عملية الإضافة", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                Dim productToAdd = New Product(
                (txbAddManufacturer.Text + txbAddModel.Text + txbAddYear.Text).ToLower.Replace(" ", ""),
                txbAddDevice.Text, txbAddManufacturer.Text, txbAddModel.Text, txbAddMadeIn.Text,
                txbAddYear.Text, txbAddPrice.Text, txbAddCount.Text
                )
                db.Open()

                Using cmd As New OleDbCommand("SELECT COUNT(*) FROM Products WHERE ID = ?", db)
                    cmd.Parameters.AddWithValue("?", productToAdd.ID)
                    Dim exists = Convert.ToInt32(cmd.ExecuteScalar)
                    If exists <= 0 Then
                        Using cmdIns As New OleDbCommand("INSERT INTO Products (ID, device, manufacturer, model, madeIn, proYear, price, quantity) VALUES (?, ?, ?, ?, ?, ?, ?, ?)", db)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.ID)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.Device)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.Manufacturer)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.Model)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.MadeIn)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.Year)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.Price)
                            cmdIns.Parameters.AddWithValue("?", productToAdd.Quantity)
                            cmdIns.ExecuteNonQuery()
                            MsgBox("تمت إضافة جهاز جديد بقيد جديد الى قاعدة البيانات", MsgBoxStyle.Information, "جهاز جديد")
                        End Using
                    Else
                        Using cmdUpd As New OleDbCommand("UPDATE Products SET quantity = quantity + ? WHERE ID = ?", db)
                            cmdUpd.Parameters.AddWithValue("?", productToAdd.Quantity)
                            cmdUpd.Parameters.AddWithValue("?", productToAdd.ID)
                            cmdUpd.ExecuteNonQuery()
                            MsgBox("تم تحديث الكمية للجهاز الموجود مسبقاً", MsgBoxStyle.Information, "تحديث الكمية")
                        End Using
                    End If
                End Using
                db.Close()

                For Each control As Control In pnlAdd.Controls
                    If TypeOf control Is TextBox Then
                        control.Text = ""
                    End If
                Next
                txbAddCount.Value = 1
                txbAddPrice.Value = 5000
            End If
        Else
            MsgBox("الرجاء التأكد من ملئ الحقول المطلوبة بدقّة", MsgBoxStyle.Critical, "فشل عملية الشِراء")
        End If
    End Sub
    Private Sub btnCreateAdmin_Click(sender As Object, e As EventArgs) Handles btnCreateAdmin.Click
        If checkInput(pnlCreateAdmin) Then
            If chkCreateAgree.Checked Then
                If txbCreatePass.Text = txbCreateConfPass.Text Then
                    Dim result = MessageBox.Show("هل أنت متأكد من الرغبة بالتقدم؟", "تأكيد عملية الإنشاء", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Dim email = txbCreateEmail.Text
                        Dim password = txbCreatePass.Text
                        Dim fullName = txbCreateFName.Text + " " + txbCreateLName.Text
                        Dim phone = txbCreatePhone.Text
                        Dim address = txbCreateAddress.Text
                        Dim dateCreated = Now.ToString("yyyy-MM-dd HH:mm")
                        Dim createdBy = adminInControl
                        db.Open()
                        Using cmd As New OleDbCommand("SELECT COUNT(*) FROM Admins WHERE ID = ?", db)
                            cmd.Parameters.AddWithValue("?", email)
                            Dim exists = Convert.ToInt32(cmd.ExecuteScalar)
                            If exists <= 0 Then
                                Using cmdIns As New OleDbCommand("INSERT INTO Admins (ID, phoneNumber, address, createdBy, fullName, dateCreated, passwordHash) VALUES (?, ?, ?, ?, ?, ?, ?)", db)
                                    cmdIns.Parameters.AddWithValue("?", email)
                                    cmdIns.Parameters.AddWithValue("?", phone)
                                    cmdIns.Parameters.AddWithValue("?", address)
                                    cmdIns.Parameters.AddWithValue("?", createdBy)
                                    cmdIns.Parameters.AddWithValue("?", fullName)
                                    cmdIns.Parameters.AddWithValue("?", dateCreated)
                                    cmdIns.Parameters.AddWithValue("?", hashPassword(password))
                                    cmdIns.ExecuteNonQuery()
                                    MsgBox("تم إنشاء حساب جديد ", MsgBoxStyle.Information, "تمّت العملية بنجاح")
                                End Using
                            Else
                                MsgBox("البريد الإلكتروني مرفق بحساب إداري فعلاً ، قم بالتواصل مع مدير قاعدة البيانات في حال نسيانك للرمز الخاص بك", MsgBoxStyle.Critical, "فشل إتمام العملية")
                            End If
                        End Using
                        db.Close()
                        For Each control As Control In pnlCreateAdmin.Controls
                            If TypeOf control Is TextBox Then
                                control.Text = ""
                            End If
                        Next
                        chkCreateAgree.Checked = False
                    End If
                Else
                    MsgBox("كلمة المرور و تأكيدها ليسا متطابقين", MsgBoxStyle.Critical, "فشل عملية الإنشاء")
                End If
            Else
                MsgBox("على الإداري الجديد التعهد أولاً", MsgBoxStyle.Critical, "فشل عملية الإنشاء")
            End If
        Else
            MsgBox("الرجاء التأكد من ملئ الحقول المطلوبة بدقّة", MsgBoxStyle.Critical, "فشل عملية الإنشاء")
        End If
    End Sub
    Function hashPassword(password As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim hash As Byte() = sha256.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "").ToLower()
        End Using
    End Function
    Private Sub updateLog(inOut As String)
        db.Open()
        Using cmd As New OleDbCommand("UPDATE Admins SET lastLog" & inOut & " = ? WHERE ID = ?;", db)
            cmd.Parameters.AddWithValue("?", Now.ToString("yyyy-MM-dd HH:mm"))
            cmd.Parameters.AddWithValue("?", adminInControl)
            cmd.ExecuteNonQuery()
        End Using
        db.Close()
    End Sub
    Private Sub btnLogout_Click(sender As Object, e As EventArgs) Handles btnLogout.Click
        Dim result = MessageBox.Show("هل أنت متأكد من رغبتك في تسجيل الخروج؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result = DialogResult.Yes Then
            updateLog("out")
            adminInControl = ""
            adminInControPassword = ""
            pnlLogin.Visible = True
            pnlLogin.BringToFront()
            pnlLogin.Left = -1250
            Timer2.Interval = 10
            Timer2.Start()
            setupNavBar(False)
        End If
    End Sub
    Private Sub PrintPageHandler2(sender As Object, e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = 10
        Dim rightMargin As Integer = e.PageBounds.Width - 10
        Dim font As New Font("Consolas", 10, FontStyle.Regular)
        Dim boldFont As New Font("Consolas", 10, FontStyle.Bold)
        e.Graphics.DrawString("الإداري: " & adminInControl, boldFont, Brushes.Black, rightMargin - g.MeasureString("الإداري: " & adminInControl, boldFont).Width, y)
        y += 20
        e.Graphics.DrawString("================================", font, Brushes.Black, rightMargin - g.MeasureString("================================", font).Width, y)
        y += 20
        e.Graphics.DrawString("الزبون: " & saleToHappen.CustomerID, font, Brushes.Black, rightMargin - g.MeasureString("الزبون: " & saleToHappen.CustomerID, font).Width, y)
        y += 30
        e.Graphics.DrawString("السلعة: " & saleToHappen.ProductID, font, Brushes.Black, rightMargin - g.MeasureString("السلعة: " & saleToHappen.ProductID, font).Width, y)
        y += 30
        e.Graphics.DrawString("السعر الأسمي: " & saleToHappen.Price & " د.ع", font, Brushes.Black, rightMargin - g.MeasureString("السعر الأسمي: " & saleToHappen.Price & " د.ع", font).Width, y)
        y += 30
        e.Graphics.DrawString("تأريخ الشراء: " & saleToHappen.PurchaseDate, font, Brushes.Black, rightMargin - g.MeasureString("تأريخ الشراء: " & saleToHappen.PurchaseDate, font).Width, y)
        y += 30
        e.Graphics.DrawString("السعر بالأقساط: " & saleToHappen.InstalmentPrice & " د.ع", font, Brushes.Black, rightMargin - g.MeasureString("السعر بالأقساط: " & saleToHappen.InstalmentPrice & " د.ع", font).Width, y)
        y += 30
        If saleToHappen.FirstInstalment = 0 AndAlso saleToHappen.Remaining = 0 Then
            e.Graphics.DrawString("المقدّم: " & saleToHappen.Price & " د.ع", font, Brushes.Black, rightMargin - g.MeasureString("المقدّم: " & saleToHappen.Price & " د.ع", font).Width, y)
        Else
            e.Graphics.DrawString("المقدّم: " & saleToHappen.FirstInstalment & " د.ع", font, Brushes.Black, rightMargin - g.MeasureString("المقدّم: " & saleToHappen.FirstInstalment & " د.ع", font).Width, y)
        End If

        y += 20
        e.Graphics.DrawString("================================", font, Brushes.Black, rightMargin - g.MeasureString("================================", font).Width, y)
        y += 20
        e.Graphics.DrawString("المتبقّي: " & saleToHappen.Remaining & " د.ع", font, Brushes.Black, rightMargin - g.MeasureString("المتبقّي: " & saleToHappen.Remaining & " د.ع", font).Width, y)
        y += 50
        e.Graphics.DrawString("نشكركم لزيارتكم!", font, Brushes.Black, rightMargin - g.MeasureString("نشكركم لزيارتكم!", font).Width, y)
    End Sub
    Private Sub PrintPageHandler(sender As Object, e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim y As Integer = 10
        Dim rightMargin As Integer = e.PageBounds.Width - 10
        Dim font As New Font("Consolas", 10, FontStyle.Regular)
        Dim boldFont As New Font("Consolas", 10, FontStyle.Bold)

        Dim selectedRow As DataGridViewRow = table.Rows(currentPrintIndex)

        ' Header
        e.Graphics.DrawString("الإداري: " & adminInControl, boldFont, Brushes.Black, rightMargin - g.MeasureString("الإداري: " & adminInControl, boldFont).Width, y)
        y += 20
        e.Graphics.DrawString("تأريخ التصدير: " & Now.ToString("dd/MM/yyyy HH:mm"), font, Brushes.Black, rightMargin - g.MeasureString("تأريخ التصدير: " & Now.ToString("dd/MM/yyyy HH:mm"), font).Width, y)
        y += 20
        e.Graphics.DrawString("================================", font, Brushes.Black, rightMargin - g.MeasureString("================================", font).Width, y)
        y += 20

        For Each cell As DataGridViewCell In selectedRow.Cells
            If Not IsDBNull(cell.Value) Then
                If IsNumeric(cell.Value) AndAlso cell.Value = 0 Then Continue For
                If cell.OwningColumn.Name = "adminID" Then Continue For
                If cell.OwningColumn.Name = "remaining" Then
                    If all Then
                        ttRemain += cell.Value
                        Continue For
                    Else
                        Continue For
                    End If
                End If
                Dim suffix As String = If(IsNumeric(cell.Value), "د.ع ", "")
                Dim text As String = cell.OwningColumn.HeaderText & " : " & cell.Value.ToString & suffix
                e.Graphics.DrawString(text, boldFont, Brushes.Black, rightMargin - g.MeasureString(text, boldFont).Width, y)
                y += 30
            End If
        Next

        ' Footer
        e.Graphics.DrawString("================================", font, Brushes.Black, rightMargin - g.MeasureString("================================", font).Width, y)
        y += 20
        e.Graphics.DrawString("المتبقي: " & selectedRow.Cells("remaining").Value.ToString & "د.ع ", boldFont, Brushes.Black, rightMargin - g.MeasureString("المتبقي: " & selectedRow.Cells("remaining").Value.ToString & "د.ع ", boldFont).Width, y)
        y += 30

        If all AndAlso currentPrintIndex = table.Rows.Count - 2 Then
            e.Graphics.DrawString("الدَين الكلّي: " & ttRemain & "د.ع: ", font, Brushes.Black, rightMargin - g.MeasureString("الدَين الكلّي: " & ttRemain & "د.ع ", font).Width, y)
            y += 20
        End If

        If (all AndAlso currentPrintIndex = table.Rows.Count - 2) OrElse (Not all) Then
            e.Graphics.DrawString("نشكركم لزيارتنا!", font, Brushes.Black, rightMargin - g.MeasureString("نشكركم لزيارتنا!", font).Width, y)
        End If

        ' Handle next page
        currentPrintIndex += 1
        If all AndAlso currentPrintIndex <= table.Rows.Count - 2 Then
            e.HasMorePages = True
        Else
            e.HasMorePages = False
            all = False
            last = False
            ttRemain = 0
        End If
    End Sub
    Private Sub btnManageStore_Click(sender As Object, e As EventArgs) Handles btnManageStore.Click
        LoadStoreData()
    End Sub
    Private Sub btnManageSale_Click(sender As Object, e As EventArgs) Handles btnManageSale.Click
        LoadSalesData()
    End Sub
    Private Sub LoadSalesData()
        inSales = True
        txbSearchManagement.Text = ""
        lblDayPrev = "الوارد اليومي(د.ع)"
        lblMonthPrev = "الوارد الشهري(د.ع)"
        lblDeptPrev = "الدين الكلّي(د.ع)"
        Dim data As New DataTable()
        Dim query As String = "SELECT * FROM Sales"
        db.Open()
        Using cmd As New OleDbCommand(query, db)
            Using reader As OleDbDataReader = cmd.ExecuteReader()
                data.Load(reader)
            End Using
        End Using
        db.Close()
        table.DataSource = data
        table.ReadOnly = True
        table.RowHeadersVisible = False
        table.MultiSelect = False
        table.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        table.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        table.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        table.BackgroundColor = Color.AliceBlue
        table.DefaultCellStyle.ForeColor = Color.White
        table.DefaultCellStyle.BackColor = Color.DarkBlue
        table.ColumnHeadersHeight = 50
        table.Sort(table.Columns("purchaseDate"), ComponentModel.ListSortDirection.Descending)
        table.Columns("adminID").Visible = False
        table.Columns("customerID").HeaderText = "الزبون"
        table.Columns("customerID").DefaultCellStyle.ForeColor = Color.Gold
        table.Columns("productID").HeaderText = "السلعة"
        table.Columns("price").HeaderText = "السعر الأسمي"
        table.Columns("purchaseDate").HeaderText = "تأريخ الشراء"
        table.Columns("instalmentPrice").HeaderText = "السعر بالأقساط"
        table.Columns("remaining").HeaderText = "المبلغ المتبقي"
        table.Columns("1instalment").HeaderText = "القسط 1"
        table.Columns("remaining").DefaultCellStyle.ForeColor = Color.Gold
        Dim r = 90, b = 90
        Dim values As New List(Of Decimal)
        Dim dates As New List(Of Date)
        For Each c As DataGridViewColumn In table.Columns
            Dim match1 As Match = Regex.Match(c.HeaderText, "(\d+)instalment$")
            Dim match2 As Match = Regex.Match(c.HeaderText, "(\d+)instalmentDate$")
            If match1.Success Then
                Dim number As String = match1.Groups(1).Value
                c.HeaderText = "القسط " + number
                c.DefaultCellStyle.BackColor = Color.FromArgb(r, 0, b)
                For Each row As DataGridViewRow In table.Rows
                    If Not row.IsNewRow Then
                        Dim val = row.Cells(c.Index).Value
                        If val IsNot Nothing AndAlso IsNumeric(val) AndAlso CDec(val) <> 0 Then
                            values.Add(CDec(val))
                        End If
                    End If
                Next
            ElseIf match2.Success Then
                Dim number As String = match2.Groups(1).Value
                c.HeaderText = "تأريخ القسط " + number
                c.DefaultCellStyle.BackColor = Color.FromArgb(r, 0, b)
                r -= 15
                b -= 15
                If b <= 45 Then
                    r = 90
                    b = 90
                End If
                For Each row As DataGridViewRow In table.Rows
                    If Not row.IsNewRow Then
                        Dim val = row.Cells(c.Index).Value
                        If val IsNot Nothing AndAlso Date.TryParse(val.ToString(), Nothing) Then
                            dates.Add(CDate(val))
                        End If
                    End If
                Next
            End If
        Next
        Dim todayInstalments As New List(Of Decimal)
        Dim thisMonthInstalments As New List(Of Decimal)
        For Each row As DataGridViewRow In table.Rows
            If Not row.IsNewRow Then
                Dim remainingVal = row.Cells("remaining").Value
                Dim purchaseVal = row.Cells("purchaseDate").Value
                If remainingVal IsNot Nothing AndAlso IsNumeric(remainingVal) AndAlso CDec(remainingVal) = 0 Then
                    If purchaseVal IsNot Nothing AndAlso Date.TryParse(purchaseVal.ToString(), Nothing) Then
                        Dim parsedDate As Date = CDate(purchaseVal)
                        Dim instalmentVal = row.Cells("1instalment").Value
                        If instalmentVal IsNot Nothing AndAlso IsNumeric(instalmentVal) Then
                            If parsedDate.Date = Date.Today Then
                                todayInstalments.Add(CDec(instalmentVal))
                                thisMonthInstalments.Add(CDec(instalmentVal))
                            ElseIf parsedDate.Month = Date.Today.Month AndAlso parsedDate.Year = Date.Today.Year Then
                                thisMonthInstalments.Add(CDec(instalmentVal))
                            End If
                        End If
                    End If
                End If
            End If
        Next
        Dim todayRev = 0
        Dim monthRev = 0
        For i = 0 To dates.Count - 1
            If dates(i) = Today Then
                todayRev += values(i)
                monthRev += values(i)
            ElseIf dates(i).Month = Today.Month AndAlso dates(i).Year = Today.Year Then
                monthRev += values(i)
            End If
        Next
        For Each i In todayInstalments
            todayRev += i
        Next
        For Each i In thisMonthInstalments
            monthRev += i
        Next
        Dim remainingSum As Decimal = 0
        For Each row As DataGridViewRow In table.Rows
            If Not row.IsNewRow Then
                Dim val = row.Cells("remaining").Value
                If val IsNot Nothing AndAlso IsNumeric(val) Then
                    remainingSum += CDec(val)
                End If
            End If
        Next
        lblIncome.Text = todayRev.ToString("N0")
        lblMnth.Text = monthRev.ToString("N0")
        lblDebt.Text = remainingSum.ToString("N0")
        optionsList.DrawMode = DrawMode.OwnerDrawFixed
        optionsList.ItemHeight = 40
        table.BringToFront()
        For Each ctrl As Control In pnlManagement.Controls
            ctrl.Visible = True
        Next
        table.ClearSelection()
    End Sub
    Private Sub LoadStoreData()
        inStore = True
        txbSearchManagement.Text = ""
        lblDayPrev = "الأكثر مبيعاً"
        lblMonthPrev = "العدد المباع"
        lblDeptPrev = "إعادة تخزين"
        lblDebt.Text = "إعادة تخزين"
        Dim data As New DataTable()
        Dim query As String = "SELECT * FROM Products"
        db.Open()
        Using cmd As New OleDbCommand(query, db)
            Using reader As OleDbDataReader = cmd.ExecuteReader()
                data.Load(reader)
            End Using
        End Using
        table.DataSource = data
        table.ReadOnly = True
        table.RowHeadersVisible = False
        table.MultiSelect = False
        table.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        table.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        table.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
        table.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        table.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        table.BackgroundColor = Color.AliceBlue
        table.DefaultCellStyle.ForeColor = Color.White
        table.DefaultCellStyle.BackColor = Color.DarkBlue
        table.ColumnHeadersHeight = 50
        table.Sort(table.Columns("quantity"), ComponentModel.ListSortDirection.Ascending)
        table.Columns("ID").Visible = False
        table.Columns("device").HeaderText = "الجهاز"
        table.Columns("manufacturer").HeaderText = "الشركة المصنّعة"
        table.Columns("model").HeaderText = "الموديل"
        table.Columns("model").DefaultCellStyle.ForeColor = Color.Gold
        table.Columns("madeIn").HeaderText = "صُنع في"
        table.Columns("proYear").HeaderText = "سنة الإنتاج"
        table.Columns("price").HeaderText = "السعر"
        table.Columns("price").DefaultCellStyle.Format = "N0"
        table.Columns("quantity").HeaderText = "المتوفر"
        table.Columns("quantity").DefaultCellStyle.ForeColor = Color.Gold
        Dim query2 As String = "
SELECT productID, Total
FROM (
    SELECT productID, COUNT(*) AS Total
    FROM Sales
    GROUP BY productID
) AS Counts
WHERE Total = (
    SELECT MAX(Cnt)
    FROM (
        SELECT COUNT(*) AS Cnt
        FROM Sales
        GROUP BY productID
    ) AS MaxCount
);"
        Dim most = ""
        Dim count = 0
        Dim cmd2 As New OleDbCommand(query2, db)
        Dim reader2 As OleDbDataReader = cmd2.ExecuteReader()
        If reader2.Read() Then
            most = reader2("productID").ToString()
            count = Convert.ToInt32(reader2("Total"))
        End If
        reader2.Close()
        db.Close()
        lblIncome.Text = most
        lblMnth.Text = count
        table.BringToFront()
        For Each ctrl As Control In pnlManagement.Controls
            ctrl.Visible = True
        Next
        restockList.DrawMode = DrawMode.OwnerDrawFixed
        For Each row As DataGridViewRow In table.Rows
            If Not row.IsNewRow Then
                Dim quantity As Integer = Convert.ToInt32(row.Cells("quantity").Value)
                If quantity = 0 Then
                    Dim model As String = row.Cells("model").Value.ToString()
                    restockList.Items.Add(model)
                End If
            End If
        Next
        table.ClearSelection()
    End Sub
    Private Sub saleTable_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles table.CellFormatting
        If Not table.Columns(e.ColumnIndex).Name = "proYear" Then
            If IsNumeric(e.Value) Then
                If CInt(e.Value) = 0 Then
                    e.Value = "0"
                Else
                    e.Value = FormatNumber(e.Value, 0, TriState.False, TriState.False, TriState.True)
                End If
                e.FormattingApplied = True
            End If
        End If
    End Sub
    Private Sub lblIncome_MouseEnter(sender As Object, e As EventArgs) Handles lblIncome.MouseEnter
        Swap(lblDayPrev, lblIncome.Text)
    End Sub
    Private Sub lblIncome_MouseLeave(sender As Object, e As EventArgs) Handles lblIncome.MouseLeave
        Swap(lblDayPrev, lblIncome.Text)
    End Sub
    Private Sub btnBackManagement_Click(sender As Object, e As EventArgs) Handles btnBackManagement.Click
        If inSales Then
            If Not String.IsNullOrEmpty(txbSearchManagement.Text) Then
                LoadSalesData()
            Else
                inSales = False
                For Each ctrl As Control In pnlManagement.Controls
                    If ctrl IsNot lblSale AndAlso ctrl IsNot lblStore AndAlso ctrl IsNot btnManageStore AndAlso ctrl IsNot btnManageSale Then
                        ctrl.Visible = False
                    Else
                        ctrl.BringToFront()
                    End If
                Next
            End If
        ElseIf inStore Then
            If Not String.IsNullOrEmpty(txbSearchManagement.Text) Then
                LoadStoreData()
            Else
                inStore = False
                For Each ctrl As Control In pnlManagement.Controls
                    If ctrl IsNot lblSale AndAlso ctrl IsNot lblStore AndAlso ctrl IsNot btnManageStore AndAlso ctrl IsNot btnManageSale Then
                        ctrl.Visible = False
                    Else
                        ctrl.BringToFront()
                    End If
                Next
            End If
        End If
    End Sub
    Private Sub lblMnth_MouseEnter(sender As Object, e As EventArgs) Handles lblMnth.MouseEnter
        Swap(lblMonthPrev, lblMnth.Text)
    End Sub
    Private Sub lblMnth_MouseLeave(sender As Object, e As EventArgs) Handles lblMnth.MouseLeave
        Swap(lblMonthPrev, lblMnth.Text)
    End Sub
    Private Sub lblDebt_MouseEnter(sender As Object, e As EventArgs) Handles lblDebt.MouseEnter
        If lblDebt.Text = "إعادة تخزين" Then
            If Not expanding AndAlso Not collapsing Then
                restockList.Height = 0
                restockList.Visible = True
                expanding = True
                restockList.BringToFront()
                Timer3.Interval = 10
                Timer3.Start()
            End If
        Else
            Swap(lblDeptPrev, lblDebt.Text)
        End If
    End Sub
    Private Sub lblDebt_MouseLeave(sender As Object, e As EventArgs) Handles lblDebt.MouseLeave
        If Regex.IsMatch(lblDeptPrev, "^\d+(,\d+)*$") Then
            Swap(lblDeptPrev, lblDebt.Text)
        Else
            If Not collapsing Then
                collapsing = True
                Timer3.Start()
            End If
        End If

    End Sub
    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        If expanding Then
            If restockList.Height < restockList.ItemHeight * restockList.Items.Count Then
                restockList.Top -= 5
                restockList.Height += 5
            Else
                expanding = False
                Timer3.Stop()
            End If
        ElseIf collapsing Then
            If restockList.Height > 0 Then
                restockList.Top += 5
                restockList.Height -= 5
            Else
                collapsing = False
                restockList.Visible = False
                Timer3.Stop()
            End If
        End If
    End Sub
    Private Sub restockList_DrawItem(sender As Object, e As DrawItemEventArgs) Handles restockList.DrawItem
        If e.Index >= 0 Then
            Dim item As String = restockList.Items(e.Index).ToString()
            Dim textBrush As New SolidBrush(e.ForeColor)

            ' Get the width and height of the ListBox
            Dim listBoxWidth As Integer = restockList.ClientSize.Width
            Dim listBoxHeight As Integer = e.Bounds.Height

            ' Calculate the position to center the text
            Dim textWidth As Integer = CInt(e.Graphics.MeasureString(item, e.Font).Width)
            Dim textHeight As Integer = CInt(e.Graphics.MeasureString(item, e.Font).Height)
            Dim x As Integer = (listBoxWidth - textWidth) / 2
            Dim y As Integer = (listBoxHeight - textHeight) / 2

            ' Draw the text centered in the ListBox item
            e.Graphics.DrawString(item, e.Font, textBrush, x, y)
        End If
    End Sub
    Private Sub options(sender As Object, e As DrawItemEventArgs) Handles optionsList.DrawItem
        If e.Index >= 0 Then
            e.DrawBackground()

            Dim item As String = optionsList.Items(e.Index).ToString()
            Dim textSize As SizeF = e.Graphics.MeasureString(item, e.Font)
            Dim x As Integer = e.Bounds.X + (e.Bounds.Width - CInt(textSize.Width)) \ 2
            Dim y As Integer = e.Bounds.Y + ((e.Bounds.Height - CInt(textSize.Height)) \ 2)

            Using textBrush As New SolidBrush(e.ForeColor)
                e.Graphics.DrawString(item, e.Font, textBrush, x, y)
            End Using

            e.DrawFocusRectangle()
        End If
    End Sub
    Private Sub btnSearchManagement_Click(sender As Object, e As EventArgs) Handles btnSearchManagement.Click
        Dim column As String = ""
        If inSales Then
            column = "customerID"
        ElseIf inStore Then
            column = "model"
        End If
        Dim q = txbSearchManagement.Text.Trim().ToLower()
        Dim toRemove As New List(Of DataGridViewRow)
        For Each r As DataGridViewRow In table.Rows
            If Not r.IsNewRow AndAlso Not r.Cells(column).Value?.ToString().ToLower().Contains(q) Then
                toRemove.Add(r)
            End If
        Next

        For Each r In toRemove
            table.Rows.Remove(r)
        Next
    End Sub
    Private Sub table_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles table.CellClick
        If e.RowIndex >= 0 Then
            d = New PayBox()
            table.ClearSelection()
            table.Rows(e.RowIndex).Selected = True
            selectedRow = table.Rows(e.RowIndex)
            customerIDSearch = selectedRow.Cells("customerID").Value.ToString()
            remainderPay = selectedRow.Cells("remaining").Value
            productPay = selectedRow.Cells("productID").Value.ToString
            If Not IsDBNull(selectedRow.Cells("23instalment").Value) Then
                d.txbInput.Minimum = remainderPay
            End If
            Dim mousePos = table.PointToClient(Cursor.Position)
            optionsList.Location = New Point(mousePos.X, mousePos.Y)
            optionsList.Width = 0
            optionsList.Tag = e.RowIndex
            optionsList.Visible = True
            optionsList.BringToFront()
            expanding = True
            collapsing = False
            Timer4.Interval = 10
            Timer4.Start()
        End If
    End Sub
    Private Sub listBoxPopup_SelectedIndexChanged(sender As Object, e As EventArgs) Handles optionsList.SelectedIndexChanged
        If optionsList.SelectedIndex = -1 Then Exit Sub
        Dim selectedOption = optionsList.SelectedItem.ToString()
        Dim rowIndex = CInt(optionsList.Tag)
        Dim selectedRow = table.Rows(rowIndex)
        Select Case selectedOption
            Case "تسديد"
                pay()
            Case "طباعة"
                printDoc.Print()
            Case "بحث"
                txbSearchManagement.Text = customerIDSearch
                btnSearchManagement_Click(btnSearchManagement, EventArgs.Empty)
            Case "طباعة سجل"
                printAll()
        End Select
        optionsList.ClearSelected()
        StartCollapse()
    End Sub
    Private Sub printAll()
        all = True
        txbSearchManagement.Text = customerIDSearch
        btnSearchManagement_Click(btnSearchManagement, EventArgs.Empty)
        currentPrintIndex = 0
        last = False
        printDoc.Print()
    End Sub
    Private Sub pay()
        d.StartPosition = FormStartPosition.Manual
        Dim parentCenterX = Me.Left + (Me.Width \ 2)
        Dim parentCenterY = Me.Top + (Me.Height \ 2)
        d.Location = New Point(parentCenterX - (d.Width \ 2) - 50, parentCenterY - (d.Height \ 2))
        d.txbInput.Maximum = remainderPay
        If d.ShowDialog() = DialogResult.OK Then
            If d.input >= 5000 Then

                db.Open()

                Dim query As String = "UPDATE sales SET remaining = remaining - ? WHERE customerID = ? AND productID = ? AND remaining = ?"
                Using cmd As New OleDbCommand(query, db)
                    cmd.Parameters.AddWithValue("?", d.input) ' Decrease the 'remaining' by d.input
                    cmd.Parameters.AddWithValue("?", customerIDSearch) ' customerIDSearch variable
                    cmd.Parameters.AddWithValue("?", productPay) ' productPay variable
                    cmd.Parameters.AddWithValue("?", remainderPay) ' remainderPay variable
                    cmd.ExecuteNonQuery()
                End Using

                ' Now retrieve the updated record
                Dim selectQuery As String = "SELECT * FROM sales WHERE customerID = ? AND productID = ? AND remaining = ?"
                Using cmd As New OleDbCommand(selectQuery, db)
                    cmd.Parameters.AddWithValue("?", customerIDSearch) ' customerIDSearch variable
                    cmd.Parameters.AddWithValue("?", productPay) ' productPay variable
                    cmd.Parameters.AddWithValue("?", remainderPay - d.input) ' updated remaining value
                    Using reader As OleDbDataReader = cmd.ExecuteReader()
                        If reader.Read() Then
                            For i As Integer = 0 To reader.FieldCount - 1
                                Dim colName As String = reader.GetName(i)
                                If Regex.IsMatch(colName, "\d+instalment") Then
                                    ' Extract the number from colName (the part before "instalment")
                                    Dim instalmentNumber As Integer = Convert.ToInt32(Regex.Match(colName, "\d+").Value)

                                    ' Now proceed with your existing logic using instalmentNumber
                                    If IsDBNull(reader(i)) Then
                                        Dim updateInstalmentQuery As String = $"UPDATE sales SET [{colName}] = ? WHERE customerID = ? AND productID = ?"
                                        Using updateCmd As New OleDbCommand(updateInstalmentQuery, db)
                                            updateCmd.Parameters.AddWithValue("?", CInt(d.input))
                                            updateCmd.Parameters.AddWithValue("?", customerIDSearch)
                                            updateCmd.Parameters.AddWithValue("?", productPay)
                                            updateCmd.ExecuteNonQuery()
                                        End Using

                                        ' Use the extracted instalmentNumber in the next query for instalmentDate
                                        Dim columnPrefix As String = If(instalmentNumber = 1, "", instalmentNumber.ToString())
                                        Dim updateDateQuery As String = $"UPDATE sales SET {columnPrefix}instalmentDate = ? WHERE customerID = ? AND productID = ?"
                                        Using updateDateCmd As New OleDbCommand(updateDateQuery, db)
                                            updateDateCmd.Parameters.AddWithValue("?", Now.ToString("yyyy-MM-dd HH:mm"))
                                            updateDateCmd.Parameters.AddWithValue("?", customerIDSearch)
                                            updateDateCmd.Parameters.AddWithValue("?", productPay)
                                            updateDateCmd.ExecuteNonQuery()
                                        End Using
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    End Using
                End Using



                db.Close()





                LoadSalesData()
            Else
                MsgBox("حدث خطبٌ ما", MsgBoxStyle.Critical)
            End If
        End If

    End Sub
    Private Sub StartCollapse()
        expanding = False
        collapsing = True
        Timer4.Start()
    End Sub
    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        Dim targetWidth As Integer = 200
        Dim speed As Integer = 10

        If expanding Then
            If optionsList.Width < targetWidth Then
                optionsList.Width += speed
            Else
                optionsList.Width = targetWidth
                expanding = False
                Timer4.Stop()
            End If
        ElseIf collapsing Then
            If optionsList.Width > 0 Then
                optionsList.Width -= speed
            Else
                optionsList.Width = 0
                optionsList.Visible = False
                collapsing = False
                Timer4.Stop()
            End If
        End If
    End Sub
    Sub Swap(Of T)(ByRef a As T, ByRef b As T)
        Dim temp As T = a
        a = b
        b = temp
    End Sub
End Class
Public Class Product
    Public Property ID As String
    Public Property Device As String
    Public Property Manufacturer As String
    Public Property Model As String
    Public Property MadeIn As String
    Public Property Year As Integer
    Public Property Price As Decimal
    Public Property Quantity As Integer
    Public Sub New(ID As String, device As String, manufacturer As String, model As String, madeIn As String, year As Integer, price As Decimal, quantity As Integer)
        Me.ID = ID
        Me.Device = device
        Me.Manufacturer = manufacturer
        Me.Model = model
        Me.MadeIn = madeIn
        Me.Year = year
        Me.Price = price
        Me.Quantity = quantity
    End Sub
End Class
Public Class Customer
    Public Property ID As String
    Public Property Name As String
    Public Property Address As String
    Public Property Phone As String
    Public Sub New(id As String, name As String, address As String, phone As String)
        Me.ID = id
        Me.Name = name
        Me.Address = address
        Me.Phone = phone
    End Sub
End Class
Public Class Sale
    Public Property CustomerID As String
    Public Property ProductID As String
    Public Property AdminID As String
    Public Property Price As Integer
    Public Property PurchaseDate As DateTime
    Public Property InstalmentPrice As Integer
    Public Property Remaining As Integer
    Public Property FirstInstalment As Integer
    Public Sub New(customerID As String, productID As String, adminID As String, price As Integer, purchaseDate As DateTime)
        Me.CustomerID = customerID
        Me.ProductID = productID
        Me.AdminID = adminID
        Me.Price = price
        Me.PurchaseDate = purchaseDate
        Me.InstalmentPrice = 0
        Me.Remaining = 0
        Me.FirstInstalment = 0
    End Sub
End Class