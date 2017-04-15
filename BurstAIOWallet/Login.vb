Imports System.Text
Imports System.Security.Cryptography

Public Class Login



    Private Sub BtnGeneratePass_Click(sender As Object, e As EventArgs) Handles BtnGeneratePass.Click
        Dim APICall As callAPI = New callAPI

        Randomize()
        Dim value As Integer = CInt(Int((Int16.MaxValue * Rnd()) + 1))
        Dim value2 As Integer = CInt(Int((Int32.MaxValue * Rnd()) + 1))
        Dim passphrase As String = ""
        For i As Integer = 1 To 4
            Randomize()
            value2 = CInt(Int((Int32.MaxValue * Rnd()) + 1))

            passphrase = APICall.AES_Encrypt(callAPI.CalculateMD5Hash((value * Math.PI) / i) & passphrase & value.ToString, value2 * Math.PI)
            value = CInt(Int((Int16.MaxValue * Rnd()) + 1))

        Next

        MsgBox("Password Generated" + Environment.NewLine + "Password Size: " + passphrase.Length.ToString() + Environment.NewLine + "Make sure you save this password in your password manager")
        SavenewPasswordPrompt(passphrase)

    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        BurstWallet.showassetform(TextBox1.Text)
        Me.TextBox1.Clear()
        Application.DoEvents()
        Me.Hide()
    End Sub

    Public Sub SavenewPasswordPrompt(passphrase As String)

        Dim walletname As String = InputBox("Enter A Name For This Wallet", "Create Wallet Name", "")
        Dim walletencryptioncode As String = InputBox("Enter a password to encrypt the passphrase with (Remember This Password)", "Enter Encryption Pass", "")
        If walletencryptioncode = "" Then
            MsgBox("Encrypting with Default Passcode")
            callAPI.SaveNewPassword(walletname, passphrase, "burst")

        Else
            callAPI.SaveNewPassword(walletname, passphrase, walletencryptioncode)
        End If
        Me.TextBox1.Text = passphrase
    End Sub
    Private Sub BtnGetExisting_Click(sender As Object, e As EventArgs) Handles BtnGetExisting.Click

        Dim wallet = New SelectWallet

        Dim walletDiagResult As DialogResult
        walletDiagResult = wallet.ShowDialog()
        If walletDiagResult = System.Windows.Forms.DialogResult.OK Then
            TextBox1.Text = wallet.MyTextValue
            wallet.Dispose()

        Else
            '...
        End If
    End Sub
End Class