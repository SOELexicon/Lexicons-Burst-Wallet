Imports System.Net
Imports System.Text
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Security.Cryptography
Imports System.Web


Public Class callAPI
    Public Function AES_Encrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New RijndaelManaged
        Dim Hash_AES As New MD5CryptoServiceProvider
        Dim encrypted As String = ""
        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(Encoding.Unicode.GetBytes(pass))
            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = CipherMode.ECB
            Dim DESEncrypter As ICryptoTransform = AES.CreateEncryptor
            Dim Buffer As Byte() = Encoding.Unicode.GetBytes(input)
            encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
        Return encrypted
    End Function

    Public Function AES_Decrypt(ByVal input As String, ByVal pass As String) As String
        Dim AES As New RijndaelManaged
        Dim Hash_AES As New MD5CryptoServiceProvider
        Dim decrypted As String = ""
        Try
            Dim hash(31) As Byte
            Dim temp As Byte() = Hash_AES.ComputeHash(Encoding.Unicode.GetBytes(pass))
            Array.Copy(temp, 0, hash, 0, 16)
            Array.Copy(temp, 0, hash, 15, 16)
            AES.Key = hash
            AES.Mode = CipherMode.ECB
            Dim DESDecrypter As ICryptoTransform = AES.CreateDecryptor
            Dim Buffer As Byte() = Convert.FromBase64String(input)
            decrypted = Encoding.Unicode.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length))

        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
        Return decrypted
    End Function
  

    Public Shared Function CalculateMD5Hash(input As String) As String

        Dim md5 As MD5 = System.Security.Cryptography.MD5.Create()
        Dim inputBytes As Byte() = System.Text.Encoding.ASCII.GetBytes(input)
        Dim hash As Byte() = md5.ComputeHash(inputBytes)
        Dim sb As New StringBuilder()
        For i As Integer = 0 To hash.Length - 1
            sb.Append(hash(i).ToString("X2"))
        Next

        Return sb.ToString()
    End Function

    Public Shared Function satoshiToDecimal(sat As Int64)
        If IsNothing(sat) Or IsNumeric(sat) = False Then
            Return 0
        Else
            Return (sat / 100000000)
        End If
    End Function
    Public Shared Function decimalToSatoshi(amount As Decimal)
        Dim total As Int64 = 0
        If IsNothing(amount) Or IsNumeric(amount) = False Then
            total = 0
        Else
            total = (amount * 100000000)
        End If
        Return total
    End Function



    Public Shared Function setRewardRecipient(url As String, passphrase As String, Recipient As String, feeNQT As Integer)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=setRewardRecipient")
        request.Method = "POST"
        Dim postData As String = "&recipient=" & Recipient & "&deadline=1440&secretPhrase=" & passphrase & "&feeNQT=" & (feeNQT * 100000000)
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function

    Public Shared Function getAliasByName(url As String, aliasName As String)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAlias")
        request.Method = "POST"
        Dim postData As String = "&aliasName=" & aliasName
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Sub SaveNewPassword(WalletName As String, passphrase As String, code As String)
        '  Dim xmlDataSet As System.Data.DataSet
        Dim APICall As callAPI = New callAPI
        Dim xmlDataSet As System.Data.DataSet = New System.Data.DataSet("XML DataSet")
        xmlDataSet.ReadXml("XMLFile1.xml")
        xmlDataSet.Tables("Passwords").Rows.Add(WalletName, APICall.AES_Encrypt(passphrase, code))
        xmlDataSet.Tables("Passwords").AcceptChanges()
        xmlDataSet.WriteXml("XMLFile1.xml")
    End Sub
    Public Shared Function getPasswordTable()
        Dim passworddb = New System.Data.DataSet("XML DataSet")

        passworddb.ReadXml("XMLFile1.xml")
        Dim table = passworddb.Tables("Passwords")

        Return table
    End Function
    Public Shared Function getAliases(url As String, account As String)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAliases")
        request.Method = "POST"
        Dim postData As String = "&account=" & account
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function sellAlias(url As String, account As String) 'TODO sellAlias Function Flesh out this class more
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=sellAlias")
        request.Method = "POST"
        Dim postData As String = "&account=" & account
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function setAlias(url As String, account As String) 'TODO SetAlias Function Flesh out this class more
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=setAlias")
        request.Method = "POST"
        Dim postData As String = "&account=" & account
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function getAccountId(url As String, secretPhrase As String)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAccountId")
        request.Method = "POST"
        Dim postData As String = "&secretPhrase=" & WebUtility.UrlEncode(secretPhrase)
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function getAccount(url As String, account As String)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAccount")
        request.Method = "POST"
        Dim postData As String = "&account=" & account
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function getConstants(url As String)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getConstants")
        request.Method = "POST"
        Dim postData As String = ""
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function placeAskOrder(url As String, asset As String, quantityNQT As Decimal, priceNQT As String, secretPhrase As String, feeNQT As String)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=placeAskOrder")
        request.Method = "POST"
        Dim postData As String = "&asset=" & asset & "&quantityQNT=" & (quantityNQT) & "&priceNQT=" & decimalToSatoshi(priceNQT) & "&secretPhrase=" & secretPhrase & "&feeNQT=" & decimalToSatoshi(feeNQT) & "&deadline=1440"

        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function placeBidOrder(url As String, asset As String, quantityNQT As Decimal, priceNQT As String, secretPhrase As String, feeNQT As String)
        'setRewardRecipient API Call.
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=placeBidOrder")
        request.Method = "POST"
        Dim postData As String = "&asset=" & asset & "&quantityQNT=" & (quantityNQT) & "&priceNQT=" & decimalToSatoshi(priceNQT) & "&secretPhrase=" & secretPhrase & "&feeNQT=" & decimalToSatoshi(feeNQT) & "&deadline=1440"

        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function

    Public Shared Function getAssetTrades(url As String, asset As String)
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getTrades")
        request.Method = "POST"
        Dim postData As String = "&asset=" & asset
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function getAsset(url As String, asset As String)
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAsset")
        request.Method = "POST"
        Dim postData As String = "&asset=" & asset
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function getBidOrders(url As String, asset As String)
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getBidOrders")
        request.Method = "POST"
        Dim postData As String = "&asset=" & asset
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function

    Public Shared Function getAskOrders(url As String, asset As String)
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAskOrders")
        request.Method = "POST"
        Dim postData As String = "&asset=" & asset
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function

    Public Shared Function getAccountTransactions(url As String, account As String, Optional timestamp As String = "0", Optional confirmations As String = "0")
        Try

     
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAccountTransactions")
        request.Method = "POST"
        Dim postData As String = "&account=" & account & "&timestamp=" & timestamp
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
            Return responseFromServer
        Catch ex As Exception
            Return ""

        End Try
    End Function
    Public Shared Function getAssetAccounts(url As String, asset As String)
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=getAssetAccounts")
        request.Method = "POST"
        Dim postData As String = "&asset=" & asset
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function

    Public Shared Function sendMoney(url As String, recipient As String, amountNQT As String, secretPhrase As String, feeNQT As String, Optional message As String = "", Optional encryptedMessage As Boolean = False)
        Dim serverurl As String = url
        Dim request As WebRequest = WebRequest.Create(serverurl & "burst?requestType=sendMoney")
        request.Method = "POST"
        Dim postData As String = "&recipient=" & recipient & "&amountNQT=" & decimalToSatoshi(amountNQT) & "&secretPhrase=" & WebUtility.UrlEncode(secretPhrase) & "&feeNQT=" & decimalToSatoshi(feeNQT) & "&deadline=1440"
        If message = "" Then

        ElseIf message <> "" And encryptedMessage = False Then
            postData = postData & "&message=" & message & "&messageistext=true"
        ElseIf message <> "" And encryptedMessage = True Then
            postData = postData & "&messageToEncrypt=" & message & "&messageToEncryptIsText=true"
        End If
        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
        request.ContentType = "application/x-www-form-urlencoded"
        request.ContentLength = byteArray.Length
        Dim dataStream As Stream = request.GetRequestStream()
        dataStream.Write(byteArray, 0, byteArray.Length)
        dataStream.Close()
        Dim response As WebResponse = request.GetResponse()
        dataStream = response.GetResponseStream()
        Dim reader As New StreamReader(dataStream)
        Dim responseFromServer As String = reader.ReadToEnd()
        reader.Close()
        dataStream.Close()
        response.Close()
        Return responseFromServer
    End Function
    Public Shared Function getFastestWallet(Optional timeout As Int64 = 10000)
        Static start_time As DateTime
        Static stop_time As DateTime
        Dim elapsed_time As TimeSpan
        Dim BestWalletSpeed As Int64 = 9999999999999
        Dim BestWallet As String = ""
        Dim BestWalletHeight As Int64 = 0
        Dim WalletsToTest(4) As String
        WalletsToTest(0) = "https://wallet1.burstnation.com:8125/burst?requestType=getMiningInfo"
        WalletsToTest(1) = "https://wallet2.burstnation.com:8125/burst?requestType=getMiningInfo"
        WalletsToTest(2) = "https://wallet3.burstnation.com:8125/burst?requestType=getMiningInfo"
        WalletsToTest(3) = "https://wallet4.burstnation.com:8125/burst?requestType=getMiningInfo"
        WalletsToTest(4) = "https://wallet.nixxda.ninja:8125/burst?requestType=getMiningInfo"
        For Each i In WalletsToTest
            start_time = DateTime.Now()
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Dim exceptionhit As Boolean = False
            Dim json As JObject
            Dim thiswalletheight, thiswalletspeed As Int64
            Try
                request = DirectCast(WebRequest.Create(i), HttpWebRequest)
                request.Timeout = timeout
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                Dim rawresp As String
                rawresp = reader.ReadToEnd()
                json = JObject.Parse(rawresp)
                thiswalletspeed = json.SelectToken("requestProcessingTime")
                thiswalletheight = json.SelectToken("height")
            Catch ex As Exception
                exceptionhit = True
            End Try
            If exceptionhit = True Then
                thiswalletspeed = 9999
                thiswalletheight = 0
                stop_time = DateTime.Now()
                elapsed_time = stop_time.Subtract(start_time)
                thiswalletspeed += elapsed_time.TotalSeconds()
                If BestWalletHeight < thiswalletheight Then
                    BestWalletHeight = thiswalletheight
                End If
                If thiswalletheight = BestWalletHeight And BestWalletSpeed > thiswalletspeed Then
                    BestWalletSpeed = thiswalletspeed
                    BestWallet = i
                End If
            Else
                stop_time = DateTime.Now()
                elapsed_time = stop_time.Subtract(start_time)
                thiswalletspeed += elapsed_time.TotalSeconds()
                If BestWalletHeight < thiswalletheight Then
                    BestWalletHeight = thiswalletheight
                End If
                If thiswalletheight = BestWalletHeight And BestWalletSpeed > thiswalletspeed Then
                    BestWalletSpeed = thiswalletspeed
                    BestWallet = i
                End If
            End If
        Next

        Return BestWallet.ToString()
    End Function
End Class
