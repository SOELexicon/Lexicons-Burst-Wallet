Imports System.Security.Cryptography
Imports System.Text
Imports System.Xml
Imports System.IO
Imports Newtonsoft.Json
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports DevExpress.XtraSplashScreen
Imports Octokit


Public Class BurstWallet
    Dim currentVersion As String = "0.1.40-64Bit"

    Dim p() As Process
    Dim p2() As Process
    Public Sub OpenBrowserForm(WebAddress As String, Name As String)
        Try
            Dim f = New Browser()
            f.Tag = WebAddress
            f.Text = Name
            f.MdiParent = Me
            f.Show()
        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        End Try
    End Sub
    Private Sub CheckIfLocalRunning()

        p2 = Process.GetProcessesByName("java")
        p = Process.GetProcessesByName("javaw")
        If p.Count > 0 Or p2.Count > 0 Then
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
            SplashScreenManager.Default.SetWaitFormCaption("Jumping to Local Wallet")
            OpenBrowserForm("http://127.0.0.1:8125/index.html", "Local")
        Else
            MsgBox("Wallet Not Running")
        End If
    End Sub

    Private Sub NBIPoD_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIPoD.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to PingOfBurst Faucet")
        OpenBrowserForm("http://faucet.pingofburst.win/", "PingOfBurst Faucet")
    End Sub
    Private Sub NBIAssetExplorer_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIAssetExplorer.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to Asset Explorer")
        OpenBrowserForm("http://asset.burstnation.com/", "Asset Explorer")
    End Sub
    Private Sub casino_LinkClicked_1(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIBcasino.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to Burst Casino")
        OpenBrowserForm("https://burstcasino.com", "Burst Casino")
    End Sub

    Private Sub NetExplore_LinkClicked_1(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBINetExplore.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to Network Explorer")
        OpenBrowserForm("http://network.burstnation.com:8888/", "Network Explorer")
    End Sub

    Private Sub NBINixxdaWallet_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBINixxdaWallet.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to Nixxda Wallet")
        OpenBrowserForm("https://wallet.nixxda.ninja:8125/index.html", "Nixxda Wallet")
    End Sub
    Private Sub NBICanada1LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIBNWallet1.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to BN Canada 1")
        OpenBrowserForm("https://wallet1.burstnation.com:8125/index.html", "BN Canada 1")
    End Sub

    Private Sub NavBarItem2_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBILocal.LinkClicked
        Try
            CheckIfLocalRunning()
        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        End Try
    End Sub

    Private Sub NBIBNUsa_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIBNUsa.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to BN USA")
        OpenBrowserForm("https://wallet4.burstnation.com:8125/index.html", "BN USA")
    End Sub

    Private Sub NBIBNWallet3_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIBNWallet3.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to BN France")
        OpenBrowserForm("https://wallet3.burstnation.com:8125/index.html", "BN France")
    End Sub

    Private Sub NBIBNWallet2_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIBNWallet2.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to BN Canada 2")
        OpenBrowserForm("https://wallet2.burstnation.com:8125/index.html", "BN Canada 2")
    End Sub

    Private Sub NBINationFaucet_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBINationFaucet.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to BN Faucet")
        OpenBrowserForm("http://faucet.burstnation.com/", "BN Faucet")
    End Sub

    Private Sub NBIBCinfoFaucet_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs)
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Jumping to Faucet Burstcoin.info")
        OpenBrowserForm("https://faucet.burstcoin.info/", "Faucet Burstcoin.info")
    End Sub

    Private Sub NBIMine_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIMine.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Feeding Miner")
        ChooseMiner.Show()
        ChooseMiner.loaditems()
    End Sub
    Shared Sub RunCommandCom(command As String, arguments As String, permanent As Boolean)
        Try


            Dim p As Process = New Process()
            Dim pi As ProcessStartInfo = New ProcessStartInfo()
            pi.Arguments = " " + If(permanent = True, "/K", "/C") + " " + command + " " + arguments
            pi.FileName = "cmd.exe"
            pi.WindowStyle = ProcessWindowStyle.Hidden
            pi.CreateNoWindow = True
            p.StartInfo = pi
            p.Start()

        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        End Try
    End Sub
    Private Sub createNode(ByVal WalletName As String, ByVal WalletPass As String, ByVal writer As XmlTextWriter)

        writer.WriteStartElement("WalletName")
        writer.WriteString(WalletName)
        writer.WriteEndElement()
        writer.WriteStartElement("WalletPass")
        writer.WriteString(WalletPass)
        writer.WriteEndElement()
    End Sub

  
    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal vkey As Long) As Integer

    Public Async Sub checkrelease()
        Try

            Dim github = New GitHubClient(New ProductHeaderValue("BurstApp"))
            Dim user = Await github.Repository.Release.GetLatest(85492894)


            If currentVersion <> user.TagName Then
                Dim result As Integer = MessageBox.Show("New Release Visit Site? " + Environment.NewLine + user.Body, "New Release: " + user.TagName, MessageBoxButtons.YesNoCancel)
                If result = DialogResult.Cancel Then

                ElseIf result = DialogResult.No Then

                ElseIf result = DialogResult.Yes Then

                    Process.Start(user.HtmlUrl)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
           
        End Try

    End Sub
    Dim fastestwallet As String
    Private Sub Bursrt_AIO_Wallet_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Interval = 1
        Timer1.Enabled = True

        checkrelease()
        If (File.Exists("XMLFile1.xml") = True) Then

        Else
            Dim writer As New XmlTextWriter("XMLFile1.xml", System.Text.Encoding.UTF8)
            writer.WriteStartDocument(True)
            writer.Formatting = System.Xml.Formatting.Indented
            writer.Indentation = 2
            writer.WriteStartElement("Passwords")
            createNode("", "", writer)
            writer.WriteEndElement()
            writer.WriteEndDocument()
            writer.Close()
        End If
        fastestwallet = callAPI.getFastestWallet()
        Dim wallet As String = fastestwallet.Replace("/burst?requestType=getMiningInfo", "/index.html")


        Dim result As Integer = MessageBox.Show("Run Local Wallet? - please wait until the local wallet has loaded before pressing the local wallet link", "Run Local Wallet?", MessageBoxButtons.YesNoCancel)
        If result = DialogResult.Cancel Then

        ElseIf result = DialogResult.No Then

        ElseIf result = DialogResult.Yes Then
            RunCommandCom("run.bat", "/W", False)
        End If

        OpenBrowserForm(wallet, "Fastest Found Wallet")

    End Sub



    Private Sub BarButtonItem1_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem1.ItemClick
        PasswordManager.Show()
    End Sub

    Private Sub NBIPlotter_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIPlotter.LinkClicked
        Try

            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
            SplashScreenManager.Default.SetWaitFormCaption("Charging Plotter")
            Plot_Files.Show()
        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        End Try
    End Sub


    Private Sub NBIFastestWallet_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIFastestWallet.LinkClicked

        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Obtaining Fastest Wallet")
        Dim wallet As String = callAPI.getFastestWallet()
        wallet = wallet.Replace("/burst?requestType=getMiningInfo", "/index.html")
        OpenBrowserForm(wallet, "Fastest Found Wallet")


    End Sub

    Private Sub ClipboardClearTimer_Tick(sender As Object, e As EventArgs) Handles ClipboardClearTimer.Tick
        Clipboard.Clear()
        ClipboardClearTimer.Enabled = False
    End Sub

    Private Sub NBIBurstNation_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIBurstNation.LinkClicked

        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Fleeing To Burst Nation Forum")
        OpenBrowserForm("https://www.burstnation.com/", "Burst Nation Forums")

    End Sub
    Public Sub showassetform(passphrase As String)

        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        Catch ex As Exception

        End Try
        Try
            Dim f = New Asset
            Dim json As JObject = JObject.Parse(callAPI.getAccountId(fastestwallet.Replace("/burst?requestType=getMiningInfo", "/"), passphrase))
            f.BurstID = json.SelectToken("account").ToString()
            f.BurstRS = json.SelectToken("accountRS").ToString()
            f.BurstPublicKey = json.SelectToken("publicKey").ToString()
            f.walletaddress = fastestwallet
            f.MdiParent = Me
            f.Show()
        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        End Try
    End Sub
    Private Sub NavBarItem2_LinkClicked_1(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NavBarItem2.LinkClicked
        Try
            Login.Show()

        Catch ex As Exception
            MsgBox("Error: " + ex.ToString())
        End Try
    End Sub

    Private Sub NBIBlockExplorer_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBIBlockExplorer.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Exploring Blocks")
        OpenBrowserForm("http://explorer.burstnation.com/", "Block Explorer")

    End Sub

    Private Sub NBILotto_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles NBILotto.LinkClicked
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Randomizing Numbers")
        OpenBrowserForm("http://burst.lexitoshi.uk/ui/html/LotteryAT.html", "Burst Lottery AT")
    End Sub

    Private Sub BurstWallet_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MyBase.KeyPress
    
    End Sub
    Public Sub quickpassword()
        Dim wallet = New SelectWallet

        Dim walletDiagResult As DialogResult
        walletDiagResult = wallet.ShowDialog()
        If walletDiagResult = System.Windows.Forms.DialogResult.OK Then
            Clipboard.SetText(wallet.MyTextValue)
            wallet.Dispose()
        Else
            '...
        End If
    End Sub
    Private Sub BurstWallet_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.OemBackslash AndAlso e.Control Then
        

        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Me.ContainsFocus = True Then
            Dim controlky As Boolean
            Dim shiftkey As Boolean
            Dim keya As Boolean
            '  Dim akey As Integer

            'akey = GetAsyncKeyState(Keys.Alt)
            keya = GetAsyncKeyState(Keys.Menu)

            controlky = GetAsyncKeyState(Keys.ControlKey)
            shiftkey = GetAsyncKeyState(Keys.ShiftKey)


            If controlky And shiftkey And keya = True Then

                quickpassword()
            End If

        End If

    End Sub
End Class