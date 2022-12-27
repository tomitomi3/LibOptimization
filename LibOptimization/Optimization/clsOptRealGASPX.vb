﻿Imports LibOptimization.Util
Imports LibOptimization.MathTool

Namespace Optimization
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' SPX(Simplex Crossover) + JGG
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Cross over algorithm is SPX(Simplex Crossover).
    '''  -Alternation of generation algorithm is JGG.
    ''' 
    ''' Refference:
    ''' 樋口 隆英, 筒井 茂義, 山村 雅幸, "実数値GAにおけるシンプレクス交叉", 人工知能学会論文誌Vol. 16 (2001) No. 1 pp.147-155
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    <Serializable>
    Public Class clsOptRealGASPX : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:20,000)</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>Epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.00000001

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>Upper bound(limit solution space)</summary>
        Public Property UpperBounds As Double() = Nothing

        ''' <summary>Lower bound(limit solution space)</summary>
        Public Property LowerBounds As Double() = Nothing

        '-------------------------------------------------------------------
        'Coefficient of GA
        '-------------------------------------------------------------------
        ''' <summary>Population Size(Default:n*33)</summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>Children Size(Default:n*10)</summary>
        Public Property ChildrenSize As Integer = 100

        ''' <summary>expantion rate(Default:sqrt(n+2))</summary>
        Public Property Alpha As Double = 0.0

        ''' <summary>population</summary>
        Private m_parents As New List(Of clsPoint) 'Parent
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Optimize Function</param>
        ''' <remarks>
        ''' "n" is function dimension.
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func

            Me.PopulationSize = Me.m_func.NumberOfVariable * 33
            Me.ChildrenSize = Me.m_func.NumberOfVariable * 10
            Me.Alpha = Math.Sqrt(Me.m_func.NumberOfVariable + 2) 'recommend
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_parents.Clear()

                'check initialposition
                If MyBase.InitialPosition IsNot Nothing Then
                    If MyBase.InitialPosition.Length = MyBase.m_func.NumberOfVariable Then
                        'nothing
                    Else
                        Throw New ArgumentException("The number of variavles in InitialPosition and objective function are different.")
                    End If
                End If

                'initial position
                For i As Integer = 0 To Me.PopulationSize - 1
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_parents.Add(New clsPoint(Me.m_func, array))
                Next

                'Sort Evaluate
                Me.m_parents.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_parents.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_parents.Count OrElse Me.HigherNPercentIndex >= Me.m_parents.Count Then
                    Me.HigherNPercentIndex = Me.m_parents.Count - 1
                End If

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Do Iteration
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
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'Sort Evaluate
                Me.m_parents.Sort()

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_parents(0).Eval, Me.m_parents(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'SPX with JGG
                'Parent is n+1
                Dim parents As List(Of KeyValuePair(Of Integer, clsPoint)) = Me.SelectParent(Me.m_parents, Me.m_func.NumberOfVariable + 1)

                'Crossover
                Dim children As List(Of clsPoint) = Me.CrossOverSPX(Me.ChildrenSize, parents)

                'Replace
                Dim index As Integer = 0
                For Each p As KeyValuePair(Of Integer, clsPoint) In parents
                    Me.m_parents(p.Key) = children(index)
                    index += 1
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Select Parent
        ''' </summary>
        ''' <param name="ai_population"></param>
        ''' <param name="ai_parentSize"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function SelectParent(ByVal ai_population As List(Of clsPoint), ByVal ai_parentSize As Integer) As List(Of KeyValuePair(Of Integer, clsPoint))
            Dim ret As New List(Of KeyValuePair(Of Integer, clsPoint))

            'Index
            Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(ai_population.Count)

            'PickParents
            For i As Integer = 0 To ai_parentSize - 1
                ret.Add(New KeyValuePair(Of Integer, clsPoint)(randIndex(i), ai_population(randIndex(i))))
            Next

            Return ret
        End Function

        ''' <summary>
        ''' Simplex Crossover
        ''' </summary>
        ''' <param name="ai_childSize"></param>
        ''' <param name="ai_parents"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CrossOverSPX(ByVal ai_childSize As Integer,
                                      ByVal ai_parents As List(Of KeyValuePair(Of Integer, clsPoint))) As List(Of clsPoint)
            'Calc Centroid
            Dim g As New DenseVector(MyBase.m_func.NumberOfVariable)
            For Each p As KeyValuePair(Of Integer, clsPoint) In ai_parents
                g += p.Value
            Next
            g /= ai_parents.Count 'sum(xi)/(n+k)

            'SPX
            Dim retChilds As New List(Of clsPoint)

            'CrossOver
            For i As Integer = 0 To ai_childSize - 1
                Dim c_k As New List(Of DenseVector)
                Dim x_k As New List(Of DenseVector)
                Dim k As Integer = 0
                For Each p_k As KeyValuePair(Of Integer, clsPoint) In ai_parents
                    x_k.Add(g + Alpha * (p_k.Value - g))

                    If k = 0 Then
                        c_k.Add(New DenseVector(MyBase.m_func.NumberOfVariable)) 'all zero
                    Else
                        Dim rk As Double = m_rand.NextDouble() ^ (1 / k)
                        'If (k-1) >= Me.m_func.NumberOfVariable Then
                        '    rk = 1 'do not calc
                        'End If
                        Dim pos = rk * (x_k(k - 1) - x_k(k) + c_k(k - 1))
                        c_k.Add(pos)
                    End If
                    k += 1
                Next
                Dim n = MyBase.m_func.NumberOfVariable
                Dim tempChild As clsPoint = Nothing
                tempChild = New clsPoint(MyBase.m_func, x_k(n) + c_k(n))

                'limit solution space
                clsUtil.LimitSolutionSpace(tempChild, Me.LowerBounds, Me.UpperBounds)

                retChilds.Add(tempChild)
            Next
            retChilds.Sort() 'for tournement selection

            Return retChilds
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                Return clsUtil.FindCurrentBestPointFromPoints(m_parents, True)
            End Get
        End Property

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
                Return Me.m_parents
            End Get
        End Property
#End Region

#Region "Private Methods"
#End Region
    End Class

End Namespace
