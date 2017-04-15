Imports System.Net
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports DevExpress.XtraCharts
Imports DevExpress.XtraSplashScreen

Public Class Asset
    Public walletaddress As String

    Dim burstEpoch As New DateTime(2014, 8, 11, 4, 0, 0)
    Public BurstRS As String
    Public BurstID As String
    Public BurstPublicKey As String

    Dim xmlDataSet As System.Data.DataSet

    Private Sub Asset_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SplashScreenManager.Default.SetWaitFormCaption("Loading Local Asset DB")
        Application.DoEvents()
        xmlDataSet = New System.Data.DataSet("XML DataSet")
        ' Load the XML document to the DataSet
        xmlDataSet.ReadXml("Assets.xml")
        GridControl1.DataSource = xmlDataSet.Tables("asset")
        SplashScreenManager.Default.SetWaitFormCaption("Retreiving Fastest Wallet")

        ''   walletaddress = callAPI.getFastestWallet(5000)
        SplashScreenManager.Default.SetWaitFormCaption("Creating Candlestick Datatable")

        assetinview = New DataTable("Assets")
        With assetinview
            .Columns.Add(New DataColumn("Date", GetType(Date)))
            .Columns.Add(New DataColumn("Low", GetType(Decimal)))
            .Columns.Add(New DataColumn("High", GetType(Decimal)))
            .Columns.Add(New DataColumn("Open", GetType(Decimal)))
            .Columns.Add(New DataColumn("Close", GetType(Decimal)))
            .Columns.Add(New DataColumn("Volume", GetType(Double)))
        End With
        SplashScreenManager.Default.SetWaitFormCaption("Creating Buy Order Datatable")

        assetbuys = New DataTable("AssetBuys")
        With assetbuys
            .Columns.Add(New DataColumn("Account", GetType(String)))
            .Columns.Add(New DataColumn("Qty", GetType(Double)))
            .Columns.Add(New DataColumn("Price", GetType(Decimal)))
            .Columns.Add(New DataColumn("Total", GetType(Decimal)))
            .Columns.Add(New DataColumn("height", GetType(Integer)))
        End With
        SplashScreenManager.Default.SetWaitFormCaption("Creating Buy Sell Datatable")

        assetSells = New DataTable("AssetSells")
        With assetSells
            .Columns.Add(New DataColumn("Account", GetType(String)))
            .Columns.Add(New DataColumn("Qty", GetType(Double)))
            .Columns.Add(New DataColumn("Price", GetType(Decimal)))
            .Columns.Add(New DataColumn("Total", GetType(Decimal)))
            .Columns.Add(New DataColumn("height", GetType(Integer)))
        End With
        SplashScreenManager.Default.SetWaitFormCaption("Setting Burst ID And AccountRS")

        TEAccountID.Text = BurstID
        TEAccountRS.Text = BurstRS
        SplashScreenManager.Default.SetWaitFormCaption("Invoking getAccountAPI Call")

        Dim json As JObject = JObject.Parse(callAPI.getAccount(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), BurstID))
        TEName.Text = json.SelectToken("name").ToString()
        TEBalance.Text = (Int64.Parse(json.SelectToken("effectiveBalanceNXT").ToString()) / 100000000)
        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Try
            Dim jsonAssetInfo As JObject = JObject.Parse(callAPI.getAsset(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), TxtAddAsset.Text))
            Dim decimals As Integer = CType(jsonAssetInfo("decimals"), Integer)
            Dim assetQNTQtyTotal As Int64 = CType(jsonAssetInfo("quantityQNT"), Int64)
            Dim assetQtyTotal As Decimal = assetQNTQtyTotal / Math.Pow(10, decimals)
            Dim assetName = CType(jsonAssetInfo("name"), String)

            xmlDataSet.Tables("asset").Rows.Add(TxtAddAsset.Text, assetName, assetQtyTotal.ToString)

            xmlDataSet.Tables("asset").AcceptChanges()
            xmlDataSet.WriteXml("Assets.xml")
            TxtAddAsset.Text = ""
        Catch ex As Exception

        End Try
   
    End Sub

    Dim assetinview As DataTable
    Dim assetbuys As DataTable
    Dim assetSells As DataTable
    Dim address As String

    Private Sub updateAssetTable(timestamp As Date, value As Decimal, position As String)
        Select Case position
            Case "Low"
                For i As Integer = 0 To assetinview.Rows.Count - 1

                    If assetinview.Rows(i)("Date").ToShortDateString = timestamp.ToShortDateString Then

                        assetinview.Rows(i)("Low") = value
                    End If
                Next
            Case "High"
                For i As Integer = 0 To assetinview.Rows.Count - 1

                    If assetinview.Rows(i)("Date").ToShortDateString = timestamp.ToShortDateString Then

                        assetinview.Rows(i)("High") = value
                    End If
                Next
            Case "Open"
                For i As Integer = 0 To assetinview.Rows.Count - 1

                    If assetinview.Rows(i)("Date").ToShortDateString = timestamp.ToShortDateString Then

                        assetinview.Rows(i)("Open") = value
                    End If
                Next
            Case "Volume"

                For i As Integer = 0 To assetinview.Rows.Count - 1

                    If assetinview.Rows(i)("Date").ToShortDateString = timestamp.ToShortDateString Then

                        assetinview.Rows(i)("Volume") = value
                    End If
                Next
            Case "Close"
                For i As Integer = 0 To assetinview.Rows.Count - 1

                    If assetinview.Rows(i)("Date").ToShortDateString = timestamp.ToShortDateString Then
                        assetinview.Rows(i)("Close") = value
                    End If
                Next
        End Select
    End Sub
  
    Private Sub GenerateAssetCandles(assetID As String)
        Dim valuemembers(3) As String
        valuemembers(0) = "Low"
        valuemembers(1) = "High"
        valuemembers(2) = "Open"
        valuemembers(3) = "Close"
        Dim timestamp As Date = Now().AddDays(2)
        Dim high, low, price, largestvalue As Decimal
        Dim volume As Decimal
        assetinview.Clear()
        Dim rss As JObject = JObject.Parse(callAPI.getAssetTrades(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), assetID))
        For i As Integer = 0 To rss("trades").Count - 1
            price = (CType(rss("trades")(i)("priceNQT"), Int64) / 100000000) * Math.Pow(10, (CType(rss("trades")(i)("decimals"), Int64)))
            If timestamp.ToShortDateString <> burstEpoch.AddSeconds(CType(rss("trades")(i)("timestamp").ToString, Int64)).ToShortDateString Then
                timestamp = burstEpoch.AddSeconds(CType(rss("trades")(i)("timestamp").ToString, Int64))
                volume = (CType(rss("trades")(i)("quantityQNT"), Int64)) / Math.Pow(10, (CType(rss("trades")(i)("decimals"), Int64)))

                assetinview.Rows.Add(timestamp, price, price, price, price, volume)
                high = price
                low = price
                If i > 0 Then
                    updateAssetTable(timestamp, _
                                     (CType(rss("trades")(i - 1)("priceNQT"), Int64) / 100000000) * Math.Pow(10, (CType(rss("trades")(i)("decimals"), Int64))), _
                                     "Close")
                End If
                If i = rss("trades").Count - 1 Then
                    updateAssetTable(timestamp, price, "Open")
                End If
            Else
                timestamp = burstEpoch.AddSeconds(CType(rss("trades")(i)("timestamp").ToString, Int64))
                volume += (CType(rss("trades")(i)("quantityQNT"), Int64)) / Math.Pow(10, (CType(rss("trades")(i)("decimals"), Int64)))
                updateAssetTable(timestamp, volume, "Volume")
                If price <= low Then
                    low = price
                    updateAssetTable(timestamp, low, "Low")

                End If
                If price >= high Then
                    high = price
                    updateAssetTable(timestamp, high, "High")

                End If
                If price > largestvalue Then
                    largestvalue = price
                End If
                If i = rss("trades").Count - 1 Then
                    updateAssetTable(timestamp, price, "Open")
                End If
            End If
            '  MsgBox(rss("trades")(i).ToString() + " " + timestamp.ToLongDateString)
        Next


        GridControl2.DataSource = assetinview
        ' ChartControl1.DataSource = assetinview
        'ChartControl1.Series.Clear()
        With ChartControl1
            .Series(0).DataSource = assetinview
            .Series(0).ArgumentDataMember = "Date"
            .Series(0).ArgumentScaleType = ScaleType.DateTime
            .Series(1).DataSource = assetinview
            .Series(1).ArgumentDataMember = "Date"
            .Series(1).ArgumentScaleType = ScaleType.DateTime
        End With
    End Sub
    Dim Results(5) As String

    Private Function GetAssetInfo(assetID As String)

        Dim jsonAssetInfo As JObject = JObject.Parse(callAPI.getAsset(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), assetID))
        Dim decimals As Integer = CType(jsonAssetInfo("decimals"), Integer)
        Dim assetQNTQtyTotal As Int64 = CType(jsonAssetInfo("quantityQNT"), Int64)
        Dim accountRS As String = CType(jsonAssetInfo("accountRS"), String)
        Dim assetQtyTotal As Decimal = assetQNTQtyTotal / Math.Pow(10, decimals)
        '   Dim assetQtyTotal As Decimal = assetQNTQtyTotal / Math.Pow(10, decimals)

        Results(0) = decimals.ToString
        Results(1) = assetQtyTotal
        Results(2) = accountRS
        Results(3) = CType(jsonAssetInfo("name"), String)
        Results(4) = CType(jsonAssetInfo("numberOfAccounts"), String)
        Results(5) = CType(jsonAssetInfo("numberOfTrades"), String)


        Return Results

    End Function
    Private Sub GetAssetBuysAndSells(assetID As String, decimals As Integer)
        Dim jsonbuys As JObject = JObject.Parse(callAPI.getBidOrders(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), assetID))
        assetbuys.Clear()
        For i As Integer = 0 To jsonbuys("bidOrders").Count - 1
            Dim quantity As Int64 = CType(jsonbuys("bidOrders")(i)("quantityQNT"), Int64) / Math.Pow(10, decimals)
            Dim priceNQT As Decimal = (CType(jsonbuys("bidOrders")(i)("priceNQT"), Int64) / 100000000) * Math.Pow(10, decimals)
            Dim accountRS As String = CType(jsonbuys("bidOrders")(i)("accountRS"), String)
            Dim costTotal As Decimal = priceNQT * quantity
            Dim height As Integer = CType(jsonbuys("bidOrders")(i)("height"), Integer)
            assetbuys.Rows.Add(accountRS, quantity, priceNQT, costTotal, height)
            GridAssetBuy.DataSource = assetbuys
        Next
        Dim jsonsells As JObject = JObject.Parse(callAPI.getAskOrders(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), assetID))
        assetSells.Clear()

        For i As Integer = 0 To jsonsells("askOrders").Count - 1
            Dim quantity As Int64 = CType(jsonsells("askOrders")(i)("quantityQNT"), Int64) / Math.Pow(10, decimals)
            Dim priceNQT As Decimal = (CType(jsonsells("askOrders")(i)("priceNQT"), Int64) / 100000000) * Math.Pow(10, decimals)
            Dim accountRS As String = CType(jsonsells("askOrders")(i)("accountRS"), String)
            Dim costTotal As Decimal = priceNQT * quantity
            Dim height As Integer = CType(jsonsells("askOrders")(i)("height"), Integer)
            assetSells.Rows.Add(accountRS, quantity, priceNQT, costTotal, height)
            GridAssetSell.DataSource = assetSells
        Next
    End Sub

    Private Sub GridView1_RowClick(sender As Object, e As DevExpress.XtraGrid.Views.Grid.RowClickEventArgs) Handles GridView1.RowClick, GridView1.RowCellClick
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Fetching Asset Info")

        Dim assetID As String = GridView1.GetFocusedDataRow(0).ToString
        GetAssetInfo(assetID)
        TEAssetid.Text = assetID
        TEAssetName.Text = Results(3)
        TEAccountCount.Text = Results(4)
        TETradeCount.Text = Results(5)
        TEAssetAccountRS.Text = Results(2)

        SplashScreenManager.Default.SetWaitFormCaption("Calculating Open & Close Positions")
        GenerateAssetCandles(assetID)
        SplashScreenManager.Default.SetWaitFormCaption("Retreiving Orders")
        GetAssetBuysAndSells(assetID, CType(Results(0), Integer))
        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)

    End Sub
    Private Sub updatetotalprice() Handles TEBSQty.EditValueChanged, TSBSPrice.EditValueChanged, TEBSTxFee.EditValueChanged
        TEBSTotal.EditValue = (TEBSQty.EditValue * TSBSPrice.EditValue) + TEBSTxFee.EditValue

    End Sub
 
    Private Sub gridassetbuychange() Handles GridView3.RowClick, GridView3.RowCellClick
        TEBSQty.EditValue = CType(GridView3.GetFocusedDataRow(1), Int64)
        TSBSPrice.EditValue = CType(GridView3.GetFocusedDataRow(2), Decimal)
        updatetotalprice()
    End Sub
    Private Sub gridassetsellchange() Handles GridView4.RowClick, GridView4.RowCellClick
        TEBSQty.EditValue = CType(GridView4.GetFocusedDataRow(1), Int64)
        TSBSPrice.EditValue = CType(GridView4.GetFocusedDataRow(2), Decimal)
        updatetotalprice()
    End Sub

    Private Sub BtnBuyAsset_Click(sender As Object, e As EventArgs) Handles BtnBuyAsset.Click
        Dim wallet = New SelectWallet
        Dim passphrase As String
        Dim walletDiagResult As DialogResult
        walletDiagResult = wallet.ShowDialog()
        If walletDiagResult = System.Windows.Forms.DialogResult.OK Then
            passphrase = wallet.MyTextValue
            wallet.Dispose()

            Try
                Dim buyJson As JObject = JObject.Parse(callAPI.placeBidOrder(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), TEAssetid.Text, (TEBSQty.EditValue * Math.Pow(10, Results(0))), TSBSPrice.EditValue / Math.Pow(10, Results(0)), passphrase, TEBSTxFee.Text).ToString)

                If (CType(buyJson("broadcasted"), String) = "True") Then
                    MsgBox("Transaction Broadcasted" & Environment.NewLine & _
                           "Transaction ID: " & CType(buyJson("transaction"), String) & Environment.NewLine & _
                           "Quantity D: " & CType(buyJson("transactionJSON")("attachment")("quantityQNT"), Int64) * Math.Pow(10, Results(0)) & Environment.NewLine & _
                           "Price: " & CType(buyJson("transactionJSON")("attachment")("priceNQT"), Int64) / Math.Pow(10, Results(0)))
                Else
                    MsgBox(buyJson.ToString)
                End If

            Catch ex As Exception

            End Try
            passphrase = ""
        Else
            '...
        End If


    End Sub

    Private Sub BtnSellAsset_Click(sender As Object, e As EventArgs) Handles SimpleButton3.Click
        Dim wallet = New SelectWallet
        Dim passphrase As String
        Dim walletDiagResult As DialogResult
        walletDiagResult = wallet.ShowDialog()
        If walletDiagResult = System.Windows.Forms.DialogResult.OK Then
            passphrase = wallet.MyTextValue
            wallet.Dispose()

            Try
                Dim buyJson As JObject = JObject.Parse(callAPI.placeAskOrder(walletaddress.Replace("/burst?requestType=getMiningInfo", "/"), TEAssetid.Text, (TEBSQty.EditValue * Math.Pow(10, Results(0))), TSBSPrice.EditValue / Math.Pow(10, Results(0)), passphrase, TEBSTxFee.Text).ToString)

                If (CType(buyJson("broadcasted"), String) = "True") Then
                    MsgBox("Transaction Broadcasted" & Environment.NewLine & _
                           "Transaction ID: " & CType(buyJson("transaction"), String) & Environment.NewLine & _
                           "Quantity D: " & CType(buyJson("transactionJSON")("attachment")("quantityQNT"), Int64) * Math.Pow(10, Results(0)) & Environment.NewLine & _
                           "Price: " & CType(buyJson("transactionJSON")("attachment")("priceNQT"), Int64) / Math.Pow(10, Results(0)))
                Else
                    MsgBox(buyJson.ToString)
                End If

            Catch ex As Exception

            End Try
            passphrase = ""
        Else
            '...
        End If

    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs)

    End Sub
End Class