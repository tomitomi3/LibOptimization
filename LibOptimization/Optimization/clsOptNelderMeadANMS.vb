Imports LibOptimization.Util

Namespace Optimization
    ''' <summary>
    ''' Adaptive Nelder-Mead Simplex (ANMS) method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Also known as "Down hill simplex" or "simplex method".
    ''' 
    ''' Reffrence:
    ''' Gao, Fuchang, and Lixing Han. "Implementing the Nelder-Mead simplex algorithm with adaptive parameters." Computational Optimization and Applications 51.1 (2012): 259-277.
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    <Serializable>
    Public Class clsOptNelderMeadANMS : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:5,000)</summary>
        Public Overrides Property Iteration As Integer = 5000

        ''' <summary>Epsilon(Default:0.000001) for Criterion</summary>
        Public Property EPS As Double = 0.000001

        '-------------------------------------------------------------------
        'Coefficient of Simplex Operator
        '-------------------------------------------------------------------
        ''' <summary>Refrection coeffcient(adaptive)</summary>
        Public Property Refrection As Double = 1.0

        ''' <summary>Expantion coeffcient(adaptive)</summary>
        Public Property Expantion As Double = 2.0

        ''' <summary>Contraction coeffcient(adaptive)</summary>
        Public Property Contraction As Double = 0.5

        ''' <summary>Shrink coeffcient(adaptive)</summary>
        Public Property Shrink As Double = 0.5

        ''' <summary>vertex points(adaptive)</summary>
        Private m_points As New List(Of clsPoint)
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func

            'Adaptive
            Me.Refrection = 1.0
            Me.Expantion = 1.0 + 2.0 / Me.m_func.NumberOfVariable
            Me.Contraction = 0.75 - 1.0 / (2.0 * Me.m_func.NumberOfVariable)
            Me.Shrink = 1.0 - 1.0 / Me.m_func.NumberOfVariable
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks>
        ''' All vertexs are made at random.
        ''' </remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_points.Clear()

                'initial position
                Dim tempSimplex()() As Double = Nothing
                ReDim tempSimplex(MyBase.m_func.NumberOfVariable)
                For i As Integer = 0 To tempSimplex.Length - 1
                    tempSimplex(i) = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                Next

                Me.Init(tempSimplex)
            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT, "")
            End Try
        End Sub

        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <param name="ai_initPoint"></param>
        ''' <remarks>
        ''' Set simplex
        ''' </remarks>
        Public Overloads Sub Init(ByVal ai_initPoint()() As Double)
            Try
                'init meber varibles
                Me.m_error.Clear()
                Me.m_iteration = 0
                Me.m_points.Clear()

                'Check number of variable
                If Me.m_func.NumberOfVariable < 2 Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
                    Return
                End If

                'Check Simplex
                'MEMO:Target function variable is the same as vertex of simplex.
                If ai_initPoint.Length <> (MyBase.m_func.NumberOfVariable + 1) Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
                    Return
                End If

                'Generate vertex
                For i As Integer = 0 To m_func.NumberOfVariable
                    Me.m_points.Add(New clsPoint(MyBase.m_func, New List(Of Double)(ai_initPoint(i))))
                Next

                'Sort Evaluate
                Me.m_points.Sort()

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            Dim worstIdx = Me.m_func.NumberOfVariable
            Dim worst2ndIdx = worstIdx - 1
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'Check criterion
                Me.m_points.Sort()
                If Me.IsUseCriterion = True Then
                    If clsUtil.IsCriterion(Me.EPS, Me.m_points(0).Eval, Me.m_points(Me.m_points.Count - 1).Eval) Then
                        Return True
                    End If
                End If

#If DEBUG Then
                Dim debugCount = 0
#End If

                '-----------------------------------------------------
                'The following is optimization by Nelder-Mead Method.
                '-----------------------------------------------------
                'Reflection
                Dim temp = Util.clsUtil.GetIndexSortedEvalFromPoints(Me.m_points)
                Dim centroid = Me.GetCentroidWithoutWorst()
                Dim best = Me.m_points(temp(0).Index)
                Dim worst = Me.m_points(temp(worstIdx).Index)
                Dim worst2nd = Me.m_points(temp(worst2ndIdx).Index)
                Dim xr = New clsPoint(Me.m_func, centroid + Me.Refrection * (centroid - worst))
                If best.Eval <= xr.Eval AndAlso xr.Eval < worst2nd.Eval Then
                    'replace worst
                    Me.m_points(temp(worstIdx).Index) = xr
#If DEBUG Then
                    debugCount += 1
#End If
                End If

                'Expantion
                'temp = Util.clsUtil.GetIndexSortedEvalFromPoints(Me.m_points)
                'best = Me.m_points(temp(0).Index)
                'centroid = Me.GetCentroidWithoutWorst()
                If xr.Eval < best.Eval Then
                    Dim xe = New clsPoint(Me.m_func, centroid + Me.Expantion * (xr - centroid))
                    If xe.Eval < xr.Eval Then
                        'replace worst
                        Me.m_points(temp(worstIdx).Index) = xe
                    Else
                        'replace worst
                        Me.m_points(temp(worstIdx).Index) = xr
                    End If
#If DEBUG Then
                    debugCount += 1
#End If
                End If

                'Outside Contraction
                'temp = Util.clsUtil.GetIndexSortedEvalFromPoints(Me.m_points)
                'worst = Me.m_points(temp(worstIdx).Index)
                'worst2nd = Me.m_points(temp(worst2ndIdx).Index)
                'centroid = Me.GetCentroidWithoutWorst()
                If worst2nd.Eval <= xr.Eval AndAlso xr.Eval < worst.Eval Then
                    Dim xoc = New clsPoint(Me.m_func, centroid + Me.Contraction * (xr - centroid))
                    If xoc.Eval <= xr.Eval Then
                        'replace worst
                        Me.m_points(temp(worstIdx).Index) = xoc
                    Else
                        'Shrink without best
                        CalcShrink()
                    End If
#If DEBUG Then
                    debugCount += 1
#End If
                End If

                'Inside Contraction
                'temp = Util.clsUtil.GetIndexSortedEvalFromPoints(Me.m_points)
                'worst = Me.m_points(temp(worstIdx).Index)
                'centroid = Me.GetCentroidWithoutWorst()
                If xr.Eval >= worst.Eval Then
                    Dim xic = New clsPoint(Me.m_func, centroid - Me.Contraction * (xr - centroid))
                    If xic.Eval < worst.Eval Then
                        Me.m_points(temp(worstIdx).Index) = xic
                    Else
                        'Shrink without best
                        CalcShrink()
                    End If
#If DEBUG Then
                    debugCount += 1
#End If
                End If

#If DEBUG Then
                If debugCount > 1 Then
                    Console.WriteLine("")
                End If
#End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return clsUtil.FindCurrentBestPointFromPoints(Me.m_points, True)
            End Get
        End Property

        ''' <summary>
        ''' Get recent error infomation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLastErrorInfomation() As clsError.clsErrorInfomation
            Return Me.m_error.GetLastErrorInfomation()
        End Function

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Me.m_points
            End Get
        End Property
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Calc Centroid without worst
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCentroidWithoutWorst() As clsPoint
            Dim ret As New clsPoint(Me.m_func)
            Dim temp = Util.clsUtil.GetIndexSortedEvalFromPoints(Me.m_points)
            For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                Dim tempVal As Double = 0.0
                For j As Integer = 0 To Me.m_points.Count - 2
                    Dim idx = temp(j).Index
                    tempVal += Me.m_points(idx)(i)
                Next
                ret(i) = tempVal / (Me.m_points.Count - 1)
            Next
            ret.ReEvaluate()
            Return ret
        End Function

        ''' <summary>
        ''' Shrink(without best point)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Private Sub CalcShrink()
            'Shrink without best
            Dim temp = Util.clsUtil.GetIndexSortedEvalFromPoints(Me.m_points)
            Dim best = Me.m_points(temp(0).Index)
            For i = 1 To temp.Count - 1
                Dim idx = temp(i).Index
                Me.m_points(idx) = New clsPoint(Me.m_func, best + Shrink * (Me.m_points(idx) - best))
            Next
        End Sub
#End Region

#Region "Property(Private)"
        Private Property BestPoint() As clsPoint
            Get
                Return Me.m_points(0)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(0) = value
            End Set
        End Property

        Private Property WorstPoint() As clsPoint
            Get
                Return Me.m_points(Me.m_points.Count - 1)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(Me.m_points.Count - 1) = value
            End Set
        End Property

        Private Property Worst2ndPoint() As clsPoint
            Get
                Return Me.m_points(Me.m_points.Count - 2)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(Me.m_points.Count - 2) = value
            End Set
        End Property
#End Region
    End Class

End Namespace