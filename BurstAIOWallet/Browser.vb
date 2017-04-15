Imports CefSharp.WinForms
Imports CefSharp

Public Class Browser
    Private WithEvents browser As ChromiumWebBrowser

    Dim address As String = "https://wallet1.burstnation.com:8125/index.html"
    Dim p() As Process




    Public Sub New()
        Try
            InitializeComponent()
            CefSharp.Cef.EnableHighDPISupport()

            Dim settings As New CefSettings()
            If (CefSharp.Cef.IsInitialized) Then

            Else
                CefSharp.Cef.Initialize(New CefSettings() With { _
                 .CachePath = "Cache" _
            })
            End If

        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        End Try

    End Sub

    Private Sub Browser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try

            browser = New ChromiumWebBrowser(Me.Tag) With { _
                 .BrowserSettings = New BrowserSettings(),
            .Dock = DockStyle.Fill _
            }
            browser.BrowserSettings.Javascript = CefState.Enabled
            browser.BrowserSettings.Plugins = CefState.Enabled

            Panel1.Controls.Add(browser)
            ' Timer1.Start()

        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        Finally
            Try
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
            Catch ex As Exception

            End Try


        End Try
  
    End Sub

    Private Sub Browser_Validated(sender As Object, e As EventArgs) Handles MyBase.Validated


    End Sub

    Private Sub Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Panel1.Paint

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'browser.SetZoomLevel(-1)
        ' Timer1.Stop()

    End Sub

    Private Sub BarEditItem1_EditValueChanged(sender As Object, e As EventArgs) Handles BarEditItem1.EditValueChanged
     
        browser.SetZoomLevel(BarEditItem1.EditValue * 0.2)
    End Sub

    Private Sub BarEditItem1_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarEditItem1.ItemClick

    End Sub

    Private Sub Browser_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown, browser.KeyDown
        MsgBox(e.KeyCode)
        If e.KeyCode = Keys.OemBackslash AndAlso e.Control Then

            BurstWallet.quickpassword()

        End If
    End Sub
End Class

