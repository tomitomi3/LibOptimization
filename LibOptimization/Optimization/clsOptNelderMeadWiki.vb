Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Nelder Mead Method wikipedia ver
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Also known as "Down hill simplex" or "simplex method".
    ''' 
    ''' Reffrence:
    ''' http://ja.wikipedia.org/wiki/Nelder-Mead%E6%B3%95
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptNelderMeadWiki : Inherits absOptimization
#Region "Member"
        Private ReadOnly MAX_ITERATION As Integer = 5000
        Private ReadOnly EPS As Double = 0.000001

        'Coefficient of Simplex Operator
        Private ReadOnly COEFF_Refrection As Double = 1.0
        Private ReadOnly COEFF_Expantion As Double = 2.0
        Private ReadOnly COEFF_Contraction As Double = -0.5
        Private ReadOnly COEFF_Shrink As Double = 0.5

        Private m_points As New List(Of clsPoint)
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <param name="ai_randomRange">Optional:random range(Default 5 => -5 to 5)</param>
        ''' <param name="ai_maxIteration">Optional:Iteration(default 5000)</param>
        ''' <param name="ai_eps">Optional:Eps(default:1e-6)</param>
        ''' <param name="ai_coeffRefrection">Optional:Refrection coeffcient(default:1.0)</param>
        ''' <param name="ai_coeffExpantion">Optional:Expantion coeffcient(default:2.0)</param>
        ''' <param name="ai_coeffContraction">Optional:Contraction coeffcient(default:-0.5)</param>
        ''' <param name="ai_coeffShrink">Optional:Shrink coeffcient(default:0.5)</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, _
                       Optional ByVal ai_randomRange As Double = 5, _
                       Optional ByVal ai_maxIteration As Integer = 5000, _
                       Optional ByVal ai_eps As Double = 0.000001, _
                       Optional ByVal ai_coeffRefrection As Double = 1.0, _
                       Optional ByVal ai_coeffExpantion As Double = 2.0, _
                       Optional ByVal ai_coeffContraction As Double = -0.5, _
                       Optional ByVal ai_coeffShrink As Double = 0.5
                       )
            Me.m_func = ai_func

            Me.InitialValueRange = ai_randomRange
            Me.MAX_ITERATION = ai_maxIteration
            Me.EPS = ai_eps
            Me.COEFF_Refrection = ai_coeffRefrection
            Me.COEFF_Expantion = ai_coeffExpantion
            Me.COEFF_Contraction = ai_coeffContraction
            Me.COEFF_Shrink = ai_coeffShrink
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

                'Make simplex from random vertex
                Dim tempSimplex()() As Double = Nothing
                ReDim tempSimplex(MyBase.m_func.NumberOfVariable)
                For i As Integer = 0 To tempSimplex.Length - 1
                    ReDim tempSimplex(i)(MyBase.m_func.NumberOfVariable - 1)
                    For j As Integer = 0 To m_func.NumberOfVariable - 1
                        Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                        If MyBase.InitialPosition IsNot Nothing AndAlso MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                            value += Me.InitialPosition(j)
                        End If
                        tempSimplex(i)(j) = value
                    Next
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
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Counting Iteration
                If MAX_ITERATION <= m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                Me.m_iteration += 1

                'Check criterion
                Me.m_points.Sort()
                If clsUtil.IsCriterion(Me.EPS, Me.m_points(0).Eval, Me.m_points(Me.m_points.Count - 1).Eval) Then
                    Return True
                End If

                '-----------------------------------------------------
                'The following is optimization by Nelder-Mead Method.
                '-----------------------------------------------------
                'Calc centroid
                Dim centroid = Me.GetCentroid(m_points)

                'Reflection
                Dim refrection = Me.ModifySimplex(Me.WorstPoint, centroid, Me.COEFF_Refrection)
                If BestPoint.Eval <= refrection.Eval AndAlso refrection.Eval < Worst2ndPoint.Eval Then
                    WorstPoint = refrection
                ElseIf refrection.Eval < BestPoint.Eval Then
                    'Expantion
                    Dim expantion = Me.ModifySimplex(Me.WorstPoint, centroid, Me.COEFF_Expantion)
                    If expantion.Eval < refrection.Eval Then
                        WorstPoint = expantion
                    Else
                        WorstPoint = refrection
                    End If
                Else
                    'Contraction
                    Dim contraction = Me.ModifySimplex(WorstPoint, centroid, Me.COEFF_Contraction)
                    If contraction.Eval < WorstPoint.Eval Then
                        WorstPoint = contraction
                    Else
                        'Reduction(Shrink) BestPoint以外を縮小
                        Me.Shrink(Me.COEFF_Shrink)
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return Me.BestPoint
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
        Public Overrides ReadOnly Property ResultForDebug As List(Of clsPoint)
            Get
                Return Me.m_points
            End Get
        End Property
#End Region

#Region "Private Methods"
        ''' <summary>
        ''' Calc Centroid
        ''' </summary>
        ''' <param name="ai_vertexs"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCentroid(ByVal ai_vertexs As List(Of clsPoint)) As clsPoint
            Dim ret As New clsPoint(ai_vertexs(0))

            Dim numVar As Integer = ai_vertexs(0).Count
            For i As Integer = 0 To numVar - 1
                Dim temp As Double = 0.0
                For numVertex As Integer = 0 To ai_vertexs.Count - 2 'Except Worst
                    temp += ai_vertexs(numVertex)(i)
                Next
                ret(i) = temp / (ai_vertexs.Count - 1)
            Next

            ret.ReEvaluate()
            Return ret
        End Function

        ''' <summary>
        ''' Simplex
        ''' </summary>
        ''' <param name="ai_tgt">Target vertex</param>
        ''' <param name="ai_base">Base vertex</param>
        ''' <param name="ai_coeff">Coeffcient</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Private Function ModifySimplex(ByVal ai_tgt As clsPoint, ByVal ai_base As clsPoint, ByVal ai_coeff As Double) As clsPoint
            Dim ret As New clsPoint(Me.m_func)
            For i As Integer = 0 To ret.Count - 1
                Dim temp As Double = ai_base(i) + ai_coeff * (ai_base(i) - ai_tgt(i))
                ret(i) = temp
            Next
            ret.ReEvaluate()
            Return ret
        End Function

        ''' <summary>
        ''' Shrink(Except best point)
        ''' </summary>
        ''' <param name="ai_coeff">Shrink coeffcient</param>
        ''' <remarks>
        ''' </remarks>
        Private Sub Shrink(ByVal ai_coeff As Double)
            For i As Integer = 1 To m_points.Count - 1 'expect BestPoint
                For j As Integer = 0 To Me.m_points(0).Count - 1
                    Dim temp = BestPoint(j) + ai_coeff * (Me.m_points(i)(j) - BestPoint(j))
                    m_points(i)(j) = temp
                Next
                m_points(i).ReEvaluate()
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