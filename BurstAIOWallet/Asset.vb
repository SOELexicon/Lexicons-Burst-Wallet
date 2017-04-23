Imports System.Net
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports DevExpress.XtraCharts
Imports DevExpress.XtraSplashScreen
Imports System.Threading

Public Class Asset
    Public walletaddress, ApiAddress As String
    Private trd As Thread
    Dim burstEpoch As New DateTime(2014, 8, 11, 4, 0, 0)
    Public BurstRS As String
    Public BurstID As String
    Public BurstPublicKey As String

    Dim xmlDataSet As System.Data.DataSet
    Dim assetinview, assetbuys, assetSells, assetsowned, transactionstable, transactionschart, assetaccountsdt As DataTable
    Dim besttimestamp As Int64 = 0
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
        ApiAddress = walletaddress.Replace("/burst?requestType=getMiningInfo", "/")
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
        assetsowned = New DataTable("AssetsOwned")
        With assetsowned
            .Columns.Add(New DataColumn("AssetID", GetType(String)))
            .Columns.Add(New DataColumn("Name", GetType(String)))
            .Columns.Add(New DataColumn("Decimals", GetType(Integer)))
            .Columns.Add(New DataColumn("amountNQT", GetType(Decimal)))
            .Columns.Add(New DataColumn("Value", GetType(Decimal)))
        End With
        transactionstable = New DataTable("Transactions")
        With transactionstable
            .Columns.Add(New DataColumn("DateTime", GetType(DateTime)))
            .Columns.Add(New DataColumn("senderRS", GetType(String)))
            .Columns.Add(New DataColumn("recipientRS", GetType(String)))
            .Columns.Add(New DataColumn("Type", GetType(String)))
            .Columns.Add(New DataColumn("SubType", GetType(String)))
            .Columns.Add(New DataColumn("Sent/Receive", GetType(String)))
            .Columns.Add(New DataColumn("amountNQT", GetType(Integer)))
            .Columns.Add(New DataColumn("feeNQT", GetType(Decimal)))
            .Columns.Add(New DataColumn("timestamp", GetType(Decimal)))
            .Columns.Add(New DataColumn("TransactionID", GetType(String)))

        End With
        Dim json As JObject = JObject.Parse(callAPI.getAccount(ApiAddress, BurstID))
        Try
            TEName.Text = json.SelectToken("name").ToString()

        Catch ex As Exception
            '   MsgBox("Error Setting Name", MsgBoxStyle.Information)

        End Try
        Try
            TEBalance.Text = (Int64.Parse(json.SelectToken("effectiveBalanceNXT").ToString()) / 100000000)

        Catch ex As Exception
            ' MsgBox("Error Obtaining Balance", MsgBoxStyle.Information)
        End Try
        SplashScreenManager.Default.SetWaitFormCaption("Fetching Asset Balance's")
        Dim assettotalvalue As Decimal = 0

        Try

       
        If IsNothing(json.SelectToken("assetBalances")) = False Then
            If json.SelectToken("assetBalances").Count > 0 Then

                For i As Integer = 0 To json.SelectToken("assetBalances").Count - 1
                    Try

                        Dim balanceasset As Decimal = CType(json("assetBalances")(i)("balanceQNT"), Decimal)
                        Dim asset As String = CType(json("assetBalances")(i)("asset"), String)

                        Dim jsonAssetInfo As JObject = JObject.Parse(callAPI.getAsset(ApiAddress, asset))
                        Dim assetname As String = jsonAssetInfo("name")
                        Dim assetdecimals As Integer = jsonAssetInfo("decimals")
                        Dim assettrade As JObject = JObject.Parse(callAPI.getBidOrders(ApiAddress, asset))
                        Dim assetvalue As Decimal
                        If assettrade("bidOrders").Count > 0 Then
                            assetvalue = ((CType(assettrade("bidOrders")(0)("priceNQT"), Int64) / 100000000) * Math.Pow(10, assetdecimals)) * (balanceasset / Math.Pow(10, assetdecimals))
                        Else
                            assetvalue = 0
                        End If
                        assettotalvalue += assetvalue
                        assetsowned.Rows.Add(asset, assetname, assetdecimals, balanceasset / Math.Pow(10, assetdecimals), assetvalue)

                    Catch ex As Exception
                    End Try
                Next
                assetsowned.AcceptChanges()
            End If
            End If
        Catch ex As Exception

        End Try
        TEAssetValue.Text = assettotalvalue
        TEAccountValue.Text = CType(TEBalance.EditValue, Decimal) + CType(TEAssetValue.EditValue, Decimal)

        assetTransfersGC.DataSource = assetsowned
        ' assetTransfersGC.DataSource
        transactionschart = New DataTable("Transactions")
        With transactionschart
            .Columns.Add(New DataColumn("DateTime", GetType(DateTime)))
            .Columns.Add(New DataColumn("amountNQT", GetType(Decimal)))
        End With

        ChartControl2.DataSource = transactionschart
        GridControl4.DataSource = transactionstable
        SplashScreenManager.Default.SetWaitFormCaption("Fetching Transaction's")
        Try
            BackgroundWorker1.RunWorkerAsync(sender)

        Catch ex As Exception

        End Try

    
        DPLUEAssetID.Properties.DataSource = assetsowned
        DPLUEAssetID.Properties.DisplayMember = "AssetID"
        DPLUEAssetID.Properties.ValueMember = "AssetID"
    

        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub GenerateTransactionDataSet()


        Dim jsonTransactions As JObject = JObject.Parse(callAPI.getAccountTransactions(ApiAddress, BurstID, "0", 0))
        Dim rowTransaction As DataRow
        GridControl4.Invoke(Sub()
                                transactionstable.Clear()
                                ProgressBarControl1.Properties.Maximum = jsonTransactions("transactions").Count
                                ProgressBarControl1.EditValue = 0
                            End Sub)

        If jsonTransactions("transactions").Count > 0 Then
            For i As Integer = 0 To jsonTransactions("transactions").Count - 1
                With jsonTransactions("transactions")(i)
                    Dim fee As Int16 = CType(.SelectToken("feeNQT"), Int64) / 100000000
                    Dim senderrs As String = .SelectToken("senderRS")
                    Dim sendreceive As String = "Receive"
                    Dim recipientRS As String = .SelectToken("recipientRS")
                    Dim ammount As Decimal = CType(.SelectToken("amountNQT"), Int64) / 100000000
                    Dim timestamp As Int64 = CType(.SelectToken("timestamp"), Int64)
                    Dim transaction As String = .SelectToken("transaction").ToString
                    Dim tranTypeList As List(Of String) = getTranType(.SelectToken("type").ToString, .SelectToken("subtype").ToString)

                    Dim transactiondatetim As DateTime = burstEpoch.AddSeconds(timestamp)
                    If timestamp > besttimestamp Then
                        besttimestamp = timestamp
                    End If
                    rowTransaction = transactionstable.NewRow
                    rowTransaction("DateTime") = transactiondatetim
                    rowTransaction("senderRS") = senderrs
                    If senderrs = BurstRS Then
                        rowTransaction("Sent/Receive") = "Sent"
                        rowTransaction("amountNQT") = -ammount
                    Else
                        rowTransaction("Sent/Receive") = "Received"
                        rowTransaction("amountNQT") = ammount
                    End If
                    rowTransaction("recipientRS") = recipientRS
                    rowTransaction("feeNQT") = fee
                    rowTransaction("timestamp") = timestamp
                    rowTransaction("TransactionID") = transaction
                    rowTransaction("Type") = tranTypeList(0)
                    rowTransaction("SubType") = tranTypeList(1)
                    GridControl4.Invoke(Sub()
                                            transactionstable.Rows.Add(rowTransaction)
                                            ProgressBarControl1.EditValue += 1

                                        End Sub)




                End With


            Next
            GridControl4.Invoke(Sub()
                                    transactionstable.AcceptChanges()

                                End Sub)
        End If
    End Sub
    Private Function getTranType(typetran As Integer, subtypetran As Integer)
        Dim transactionTypes As List(Of String) = New List(Of String)

        Select Case typetran
            Case 0
                'Payments
                transactionTypes.Add("Payments")
                Select Case subtypetran
                    Case 0
                        transactionTypes.Add("Payment")



                End Select
            Case 1
                'messaging
                transactionTypes.Add("Messaging")
                Select Case subtypetran
                    Case 0
                        transactionTypes.Add("Arbitrary message")
                    Case 1
                        transactionTypes.Add("Alias assignment")
                    Case 2
                        transactionTypes.Add("Poll creation")
                    Case 3
                        transactionTypes.Add("Vote casting")
                    Case 4
                        transactionTypes.Add("Hub terminal announcement")
                    Case 5
                        transactionTypes.Add("Account info")
                    Case 6
                        transactionTypes.Add("Alias sell")
                    Case 7
                        transactionTypes.Add("Alias buy")
                End Select

            Case 2
                'Assets
                transactionTypes.Add("Assets")
                Select Case subtypetran
                    Case 0
                        transactionTypes.Add("Asset issue")
                    Case 1
                        transactionTypes.Add("Asset transfer")
                    Case 2
                        transactionTypes.Add("Ask order placement")
                    Case 3
                        transactionTypes.Add("Bid order placement")
                    Case 4
                        transactionTypes.Add("Ask order cancellation")
                    Case 5
                        transactionTypes.Add("Bid order cancellation")
                End Select
            Case 3
                'Digital Goods
                transactionTypes.Add("Digital Goods")
                Select Case subtypetran
                    Case 0
                        transactionTypes.Add("Listing")
                    Case 1
                        transactionTypes.Add("Delisting")
                    Case 2
                        transactionTypes.Add("Price change")
                    Case 3
                        transactionTypes.Add("Quantity change")
                    Case 4
                        transactionTypes.Add("Purchase")
                    Case 5
                        transactionTypes.Add("Delivery")
                    Case 6
                        transactionTypes.Add("Feedback")
                    Case 7
                        transactionTypes.Add("Refund")
                End Select
            Case 4
                'Account Control
                transactionTypes.Add("Account Control")

                Select Case subtypetran
                    Case 0
                        transactionTypes.Add("Effective balance leasing")

                End Select
            Case Else
                transactionTypes.Add("Not Listed")
                transactionTypes.Add("Not Listed")
        End Select
        Return transactionTypes
    End Function

    Private Sub GenerateTransactionChartDataSet()
      
        If transactionstable.Rows.Count > 0 Then
            ChartControl2.Invoke(Sub()
                                     transactionschart.Clear()
                                 End Sub)
            Dim chartbalance As Decimal = 0
            Dim chartdate As DateTime
            For i As Integer = 0 To transactionstable.Rows.Count - 1

                If i = 0 Then
                    chartdate = CType(transactionstable.Rows(i)(0), DateTime).ToShortDateString
                    chartbalance = transactionstable.Rows(i)("amountNQT")
                Else
                    Dim chartlocal As Integer = i

                    If chartdate <> CType(transactionstable.Rows(i)(0), DateTime).ToShortDateString Then
                        Dim priorchartdate As DateTime = transactionstable.Rows(i - 1)(0)
                        Me.Invoke(Sub()
                                      transactionschart.Rows.Add(priorchartdate.ToShortDateString, chartbalance)
                                      transactionschart.AcceptChanges()

                                  End Sub)
                        chartdate = CType(transactionstable.Rows(i)(0), DateTime).ToShortDateString
                        chartbalance = transactionstable.Rows(i)("amountNQT")
                    Else
                        chartbalance += transactionstable.Rows(i)("amountNQT")
                        Dim chartsubmissiondate As DateTime = transactionstable.Rows(i)(0)

                        If i = transactionstable.Rows.Count - 1 Then
                            ChartControl2.Invoke(Sub()
                                                     transactionschart.Rows.Add(chartsubmissiondate.ToShortDateString, chartbalance)
                                                     'transactionschart.AcceptChanges()
                                                 End Sub)
                            chartbalance = 0
                        End If

                    End If
                End If
            Next
            ChartControl2.Invoke(Sub()
                                     transactionschart.AcceptChanges()

                                 End Sub)
        End If

    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Try
            Dim jsonAssetInfo As JObject = JObject.Parse(callAPI.getAsset(ApiAddress, TxtAddAsset.Text))
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
        Dim rss As JObject = JObject.Parse(callAPI.getAssetTrades(ApiAddress, assetID))
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

        Dim jsonAssetInfo As JObject = JObject.Parse(callAPI.getAsset(ApiAddress, assetID))
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
        Dim jsonbuys As JObject = JObject.Parse(callAPI.getBidOrders(ApiAddress, assetID))
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
        Dim jsonsells As JObject = JObject.Parse(callAPI.getAskOrders(ApiAddress, assetID))
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
        GridView3.BestFitColumns()
        GridView4.BestFitColumns()

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
                Dim buyJson As JObject = JObject.Parse(callAPI.placeBidOrder(ApiAddress, TEAssetid.Text, (TEBSQty.EditValue * Math.Pow(10, Results(0))), TSBSPrice.EditValue * Math.Pow(10, Results(0)), passphrase, TEBSTxFee.Text).ToString)

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
                Dim buyJson As JObject = JObject.Parse(callAPI.placeAskOrder(ApiAddress, TEAssetid.Text, (TEBSQty.EditValue * Math.Pow(10, Results(0))), TSBSPrice.EditValue / Math.Pow(10, Results(0)), passphrase, TEBSTxFee.Text).ToString)

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

    Private Sub GetAssetAccounts_Click(sender As Object, e As EventArgs) Handles BtnGetAssetAccounts.Click
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
        SplashScreenManager.Default.SetWaitFormCaption("Executing API Call: getAssetAccounts")

        Dim assetaccounts As JObject = JObject.Parse(callAPI.getAssetAccounts(ApiAddress, DPLUEAssetID.EditValue))
        assetaccountsdt = New DataTable("assetAccounts")

        With assetaccountsdt
            .Columns.Add(New DataColumn("account", GetType(String)))
            .Columns.Add(New DataColumn("accountRS", GetType(String)))
            .Columns.Add(New DataColumn("unconfirmedQuantityQNT", GetType(String)))
            .Columns.Add(New DataColumn("quantityQNT", GetType(String)))
            .Columns.Add(New DataColumn("Include", GetType(Boolean)))
            .Columns.Add(New DataColumn("percentOwned", GetType(Decimal)))
            .Columns.Add(New DataColumn("DividendValue", GetType(Decimal)))

        End With
        SplashScreenManager.Default.SetWaitFormCaption("Parsing Asset Account Data. ")

        Dim assetdivinfo As DataRowView = DPLUEAssetID.GetSelectedDataRow
        Dim decimals As Decimal = CType(assetdivinfo("Decimals"), Integer)
        For Each i In assetaccounts("accountAssets")
            SplashScreenManager.Default.SetWaitFormDescription("Current Account: " & i("accountRS").ToString & " Shares: " & (CType(i("quantityQNT"), Int64) / Math.Pow(10, decimals)).ToString)
            If excludeaccounts <> i("accountRS").ToString Then
                assetaccountsdt.Rows.Add(i("account"), i("accountRS"), (CType(i("unconfirmedQuantityQNT"), Int64) / Math.Pow(10, decimals)), (CType(i("quantityQNT"), Int64) / Math.Pow(10, decimals)), True, 0, 0)

            End If

        Next
        assetaccountsdt.AcceptChanges()
        GridControl3.DataSource = assetaccountsdt

        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        Catch ex As Exception

        End Try

    End Sub
    Dim dividenddataset As DataTable
    Dim totalassetaccount As Int16 = 0

    Private Sub BtnCalculateDiv_Click(sender As Object, e As EventArgs) Handles BtnCalculateDiv.Click
        Try
            BtnCalculateDiv.Enabled = False
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
            SplashScreenManager.Default.SetWaitFormCaption("Calculating Asset Payments. ")

            totalassetaccount = 0
            dividenddataset = New DataTable("dividenddataset")
            Dim totalAssetamount As Decimal = 0
            Dim dividendpayout As Decimal = CType(TEDivamount.EditValue, Decimal)
            SplashScreenManager.Default.SetWaitFormCaption("Working Out Total Assets. ")

            For Each i As DataRow In assetaccountsdt.Rows
                If i("Include") Then
                    SplashScreenManager.Default.SetWaitFormDescription("Total Assets: " & totalAssetamount.ToString & " Account: " & i("accountRS").ToString)

                    totalAssetamount += i("quantityQNT")
                    totalassetaccount += 1

                End If
            Next
            SplashScreenManager.Default.SetWaitFormCaption("Calculating Percentages")

            For Each i As DataRow In assetaccountsdt.Rows
                If i("Include") Then

                    i("percentOwned") = i("quantityQNT") / totalAssetamount
                    i("DividendValue") = i("percentOwned") * (dividendpayout - totalassetaccount)
                    SplashScreenManager.Default.SetWaitFormDescription("Processed Account: " & i("accountRS").ToString & " Value: " & i("DividendValue").ToString & " Burst")

                Else
                    i("percentOwned") = 0
                    i("DividendValue") = 0
                End If
            Next
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
            BtnCalculateDiv.Enabled = True

        Catch ex As Exception

        End Try
    End Sub
    Private passphrase As String
    Private excludeaccounts As String = "BURST-NU58-Z4QR-XXKE-94DHH"
    Private Sub BtnSendPayments_Click(sender As Object, e As EventArgs) Handles BtnSendPayments.Click
        Dim result As Integer = MessageBox.Show("Process Asset Dividends", "Send Money?", MessageBoxButtons.YesNoCancel)
        If result = DialogResult.Cancel Then

        ElseIf result = DialogResult.No Then

        ElseIf result = DialogResult.Yes Then
            Try


                Dim wallet = New SelectWallet
                Dim walletDiagResult As DialogResult
                walletDiagResult = wallet.ShowDialog()
                If walletDiagResult = System.Windows.Forms.DialogResult.OK Then
                    DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(BurstWaitForm))
                    SplashScreenManager.Default.SetWaitFormCaption("Sending Asset Payments.")
                    passphrase = wallet.MyTextValue
                    wallet.Dispose()
                    If passphrase <> "" Then

                        Dim assetdivinfo As DataRowView = DPLUEAssetID.GetSelectedDataRow
                        Dim decimals As Decimal = CType(assetdivinfo("Decimals"), Integer)
                        MemoEdit1.Text = "Lexicon's burst asset dividend tool" + Environment.NewLine
                        MemoEdit1.Text += assetdivinfo("name").ToString & " Dividend Payout for Date:" & Now + Environment.NewLine
                        MemoEdit1.Text += "Sent: " + TEDivamount.Text + " Burst To " + totalassetaccount.ToString + " shareholders" + Environment.NewLine + Environment.NewLine
                        SplashScreenManager.Default.SetWaitFormDescription("Total: " + TEDivamount.Text)



                        For Each i As DataRow In assetaccountsdt.Rows
                            If i("Include") Then
                                Try
                                    SplashScreenManager.Default.SetWaitFormDescription("Total: " & TEDivamount.Text & " Payment: " & Math.Round(CType(i("DividendValue"), Decimal), 8).ToString & " To: " & i("accountRS").ToString)

                                    Dim paymentsent As JObject = JObject.Parse(callAPI.sendMoney(ApiAddress, i("account"), Math.Round(CType(i("DividendValue"), Decimal), 8), passphrase, 1))
                                    MemoEdit1.Text += "Payment sent to - accountRS: " + i("accountRS").ToString + " Value: " + i("DividendValue").ToString + " Shares: " + Math.Round(CType(i("quantityQNT").ToString, Int64) / Math.Pow(10, decimals), 8).ToString _
                                       + " TransactionID: " + paymentsent("transaction").ToString + Environment.NewLine
                                    i("Include") = False

                                Catch ex As Exception
                                    MemoEdit1.Text += "Failed sending - accountRS: " + i("accountRS").ToString + " Value: " + i("DividendValue").ToString + " Shares: " + Math.Round(CType(i("quantityQNT").ToString, Int64) / Math.Pow(10, decimals), 8).ToString + Environment.NewLine
                                End Try
                            End If
                        Next
                    End If
                    DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()

                End If

            Catch ex As Exception
            Finally
                passphrase = ""
            End Try

        End If


    End Sub
    Dim oldtimestamp As Int64
    Private Sub GetNewTransactions()
        Try
            Dim jsonTransactions As JObject = JObject.Parse(callAPI.getAccountTransactions(ApiAddress, BurstID, besttimestamp.ToString, 0))
            oldtimestamp = besttimestamp


            If jsonTransactions("transactions").Count > 0 Then
                Dim rowTransaction As DataRow

                For i As Integer = jsonTransactions("transactions").Count - 1 To 0 Step -1
                    Dim timestamp As Int64 = CType(jsonTransactions("transactions")(i).SelectToken("timestamp"), Int64)
                    If timestamp > oldtimestamp Then

                        With jsonTransactions("transactions")(i)
                            Dim fee As Int16 = CType(.SelectToken("feeNQT"), Int64) / 100000000
                            Dim senderrs As String = .SelectToken("senderRS")
                            Dim sendreceive As String = "Receive"
                            Dim recipientRS As String = .SelectToken("recipientRS")
                            Dim ammount As Decimal = CType(.SelectToken("amountNQT"), Int64) / 100000000
                            Dim transactiondatetim As DateTime = burstEpoch.AddSeconds(timestamp)
                            Dim transaction As String = .SelectToken("transaction").ToString
                            Dim alertcaption As String = ""
                            Dim alerttext As String = ""
                            Dim tranTypeList As List(Of String) = getTranType(.SelectToken("type").ToString, .SelectToken("subtype").ToString)

                            If timestamp > besttimestamp Then
                                besttimestamp = timestamp
                            End If
                            rowTransaction = transactionstable.NewRow
                            rowTransaction("DateTime") = transactiondatetim
                            rowTransaction("senderRS") = senderrs
                            If senderrs = BurstRS Then
                                rowTransaction("Sent/Receive") = "Sent"
                                rowTransaction("amountNQT") = -ammount
                            Else
                                rowTransaction("Sent/Receive") = "Received"
                                rowTransaction("amountNQT") = ammount
                            End If
                            rowTransaction("recipientRS") = recipientRS
                            rowTransaction("feeNQT") = fee
                            rowTransaction("timestamp") = timestamp
                            rowTransaction("TransactionID") = transaction
                            rowTransaction("Type") = tranTypeList(0)
                            rowTransaction("SubType") = tranTypeList(1)

                            alertcaption = rowTransaction("Sent/Receive").ToString & " " & tranTypeList(1)
                            alerttext = "From: " & senderrs & Environment.NewLine
                            alerttext += "To: " & recipientRS & Environment.NewLine
                            alerttext += "Burst: " & ammount.ToString
                            Me.Invoke(Sub()
                                          AlertTransaction.Show(Me, alertcaption, alerttext)
                                          transactionstable.Rows.InsertAt(rowTransaction, 0)
                                          transactionstable.AcceptChanges()

                                      End Sub)





                        End With

                    End If

                Next
            

            End If

        Catch ex As Exception

        End Try
    End Sub
    Dim workSender As String = "Load"
    Private Sub BackgroundWorker1_DoWork_1(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            If workSender = "Get New" Then
                GetNewTransactions()
            Else
                GenerateTransactionDataSet()
                GenerateTransactionChartDataSet()

                '  GenerateTransactionChartDataSet()
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ThreadTask()
        Try
            If workSender = "Get New" Then
                GetNewTransactions()
            Else
                GenerateTransactionDataSet()
                GenerateTransactionChartDataSet()

                '  GenerateTransactionChartDataSet()
            End If
        Catch ex As Exception

        End Try
    End Sub


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try

            trd = New Thread(AddressOf ThreadTask)
            trd.IsBackground = True
            trd.Start()
        Catch ex As Exception

        End Try
        '  GetNewTransactions()
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If workSender = "Load" Then
            workSender = "Get New"

            Timer1.Enabled = True
            Timer1.Start()
        Else

        End If

    End Sub

End Class