Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Nelder Mead Method
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Also known as "Down hill simplex" or "simplex method".
    ''' 
    ''' Reffrence:
    ''' William H. Press, Saul A. Teukolsky, William T. Vetterling, Brian P. Flannery,
    ''' "Numerical Recipes in C [日本語版] C言語による数値計算のレシピ", 平成19年第14刷, 株式会社技術評論社, pp295-299
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptNelderMeadNR : Inherits absOptimization
#Region "Member"
        Private ReadOnly MAX_ITERATION As Integer = 5000
        Private ReadOnly EPS As Double = 0.000001

        'Coefficient of Simplex Operator
        Private ReadOnly COEFF_Refrection As Double = 1.0
        Private ReadOnly COEFF_Expantion As Double = 2.0
        Private ReadOnly COEFF_Contraction As Double = 0.5
        Private ReadOnly COEFF_Shrink As Double = 2.0

        'This Parameter to use when generate a vertex
        Private ReadOnly INIT_PARAM_RANGE As Double = 5

        Private m_points As New List(Of clsPoint)

        'ErrorManage
        Private m_error As New clsError
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
        ''' <param name="ai_coeffContraction">Optional:Contraction coeffcient(default:0.5)</param>
        ''' <param name="ai_coeffShrink">Optional:Shrink coeffcient(default:2.0)</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction, _
                       Optional ByVal ai_randomRange As Double = 5, _
                       Optional ByVal ai_maxIteration As Integer = 5000, _
                       Optional ByVal ai_eps As Double = 0.000001, _
                       Optional ByVal ai_coeffRefrection As Double = 1.0, _
                       Optional ByVal ai_coeffExpantion As Double = 2.0, _
                       Optional ByVal ai_coeffContraction As Double = 0.5, _
                       Optional ByVal ai_coeffShrink As Double = 2.0
                       )
            Me.m_func = ai_func

            Me.INIT_PARAM_RANGE = ai_randomRange
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
                'Make simplex from random vertex
                Dim tempSimplex()() As Double = Nothing
                ReDim tempSimplex(MyBase.m_func.NumberOfVariable)
                For i As Integer = 0 To tempSimplex.Length - 1
                    ReDim tempSimplex(i)(MyBase.m_func.NumberOfVariable - 1)
                    For j As Integer = 0 To m_func.NumberOfVariable - 1
                        tempSimplex(i)(j) = Math.Abs(2.0 * INIT_PARAM_RANGE) * m_rand.NextDouble() - INIT_PARAM_RANGE
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

            Dim psum = Me.GetPSUM()

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.MAX_ITERATION - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Counting Iteration
                If MAX_ITERATION <= m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                'Check criterion
                Me.m_points.Sort()
                If clsUtil.IsCriterion(Me.EPS, Me.m_points(0).Eval, Me.m_points(m_points.Count - 1).Eval) Then
                    Return True
                End If

                '-----------------------------------------------------
                'The following is optimization by Nelder-Mead Method.
                '-----------------------------------------------------
                'refrection
                Dim refrection = Me.Ametory(psum, Me.WorstPoint, -1.0)
                If refrection.Eval <= Me.BestPoint.Eval Then
                    'Expantion
                    Dim expantion = Me.Ametory(psum, Me.WorstPoint, 2.0)
                    If expantion.Eval < Me.WorstPoint.Eval Then
                        Me.WorstPoint = expantion 'replace
                    End If
                ElseIf refrection.Eval >= Me.Worst2ndPoint.Eval Then
                    'contraction
                    Dim previousWorstPointEval = Me.WorstPoint.Eval
                    Dim contraction = Me.Ametory(psum, Me.WorstPoint, 0.5)
                    If contraction.Eval >= previousWorstPointEval Then
                        'shrink
                        Dim dimNum = Me.m_func.NumberOfVariable
                        Dim pNum = Me.m_points.Count
                        For i As Integer = 1 To pNum - 1  'expect BestPoint
                            For j As Integer = 0 To dimNum - 1
                                Dim temp = 0.5 * (Me.m_points(i)(j) - Me.BestPoint(j))
                                Me.m_points(i)(j) = temp
                            Next
                            Me.m_points(i).ReEvaluate()
                        Next
                        psum = Me.GetPSUM()
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Refrection, Expantion, Contraction
        ''' </summary>
        ''' <param name="psum"></param>
        ''' <param name="p"></param>
        ''' <param name="fac"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Ametory(ByVal psum As clsPoint, ByVal p As clsPoint, ByVal fac As Double) As clsPoint
            Dim temp As New clsPoint(Me.m_func)
            Dim fac1 = (1.0 - fac) / Me.m_func.NumberOfVariable
            Dim fac2 = fac1 - fac

            For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                temp(i) = psum(i) * fac1 - p(i) * fac2
            Next
            temp.ReEvaluate()

            If temp.Eval < p.Eval Then
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    psum(i) += temp(i) - p(i)
                    p(i) = temp(i)
                Next
                psum.ReEvaluate()
                p.ReEvaluate()
            End If

            Return temp
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

        Private Function GetPSUM() As clsPoint
            Dim ret As New clsPoint(Me.m_func)
            For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                For j As Integer = 0 To Me.m_points.Count - 1
                    ret(i) += Me.m_points(j)(i)
                Next
            Next
            Return ret
        End Function

        ''' <summary>
        ''' Shrink(All point replace)
        ''' </summary>
        ''' <param name="ai_allVertexs"></param>
        ''' <param name="ai_coeff">Shrink coeffcient.</param>
        ''' <remarks></remarks>
        Private Sub Shrink(ByRef ai_allVertexs As List(Of clsPoint), _
                           Optional ByVal ai_coeff As Double = 0.5)
            'Shrink. For 2 ≤ i ≤ n+1, define
            'xi = x1 + δ(xi −x1).
            Dim numVar As Integer = ai_allVertexs(0).Count

            'All point replace(Except bestpoint)
            For i As Integer = 1 To ai_allVertexs.Count - 1
                For j As Integer = 0 To numVar - 1
                    Dim temp As Double = ai_allVertexs(0)(j) + ai_coeff * (ai_allVertexs(i)(j) - ai_allVertexs(0)(j))
                    ai_allVertexs(i)(j) = temp
                Next
                ai_allVertexs(i).ReEvaluate()
            Next
        End Sub

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
                Return Me.m_points(m_points.Count - 1)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(m_points.Count - 1) = value
            End Set
        End Property

        Private Property Worst2ndPoint() As clsPoint
            Get
                Return Me.m_points(m_points.Count - 2)
            End Get
            Set(ByVal value As clsPoint)
                Me.m_points(m_points.Count - 2) = value
            End Set
        End Property
#End Region
    End Class
End Namespace
