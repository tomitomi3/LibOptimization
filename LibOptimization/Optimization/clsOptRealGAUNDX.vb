Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Real-coded Genetic Algorithm
    ''' UNDX(Unimodal Normal Distribution Crossover)
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -Alternation of generation algorithm is JGG
    ''' 
    ''' Refference:
    ''' [1]小野功，佐藤浩，小林重信, "単峰性正規分布交叉UNDXを用いた実数値GAによる関数最適化"，人工知能学会誌，Vol. 14，No. 6，pp. 1146-1155 (1999)
    ''' [2]北野 宏明 (編集), 遺伝的アルゴリズム 4, 産業図書出版株式会社, 2000年 初版, p261
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptRealGAUNDX : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:20000)</summary>
        Public Property Iteration As Integer = 20000 'generation

        ''' <summary>Epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.00000001

        ''' <summary>Use criterion</summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        '-------------------------------------------------------------------
        'Coefficient of GA
        '-------------------------------------------------------------------
        ''' <summary>Population Size(Default:n*33)</summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>Children size( = Number of CrossOver/2 )(Default:n*10)</summary>
        Public Property ChildrenSize As Integer = 100

        ''' <summary>Alpha(Default:0.5)</summary>
        Public Property ALPHA As Double = 0.5

        ''' <summary>Beta(Default:0.35)</summary>
        Public Property BETA As Double = 0.35

        ''' <summary>AlternationStrategy(Default:JGG)</summary>
        Public Property AlternationStrategy As EnumAlternatioType = EnumAlternatioType.JGG

        ''' <summary>alternation strategy</summary>
        Public Enum EnumAlternatioType
            MGG
            JGG
        End Enum

        ''' <summary>population</summary>
        Private m_parents As New List(Of clsPoint)
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

                'Set initialize value
                For i As Integer = 0 To Me.PopulationSize - 1
                    Dim temp As New List(Of Double)
                    For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                        If MyBase.InitialPosition IsNot Nothing AndAlso MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                            value += Me.InitialPosition(j)
                        End If
                        temp.Add(value)
                    Next
                    Me.m_parents.Add(New clsPoint(MyBase.m_func, temp))
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
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'Sort Evaluate
                Me.m_parents.Sort()

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_parents(0).Eval, Me.m_parents(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'Counting generation
                If Me.Iteration <= Me.m_iteration Then
                    Me.m_error.SetError(True, clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                m_iteration += 1

                'select parent
                Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(Me.m_parents.Count)
                Dim p1Index As Integer = randIndex(0)
                Dim p2Index As Integer = randIndex(1)
                Dim p1 = Me.m_parents(p1Index)
                Dim p2 = Me.m_parents(p2Index)
                Dim p3 = Me.m_parents(randIndex(2)) 'for d2

                'UNDX
                Dim children = Me.UNDX(p1, p2, p3)

                'for debug
                'If Me.m_iteration = 1 OrElse (Me.m_iteration Mod 50) = 0 Then
                '    Console.WriteLine("----")
                '    clsUtil.ToCSV(p1)
                '    clsUtil.ToCSV(p2)
                '    clsUtil.ToCSV(children)
                'End If

                'AlternationStrategy
                If Me.AlternationStrategy = EnumAlternatioType.JGG Then
                    'JGG
                    children.Sort()
                    Me.m_parents(p1Index) = children(0)
                    Me.m_parents(p2Index) = children(1)
                Else
                    'MGG
                    children.Add(p1)
                    children.Add(p2)
                    children.Sort()
                    Me.m_parents(p1Index) = children(0) 'elite
                    children.RemoveAt(0)
                    Dim rIndex = Me.SelectRoulette(children, True)
                    Me.m_parents(p2Index) = children(rIndex)
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' RouletteWheel Selection 
        ''' </summary>
        ''' <param name="ai_chidren"></param>
        ''' <param name="isForMinimize"></param>
        ''' <returns>index</returns>
        ''' <remarks></remarks>
        Private Function SelectRoulette(ByVal ai_chidren As List(Of clsPoint), ByVal isForMinimize As Boolean) As Integer
            If isForMinimize = True Then
                Dim tempSum As Double = 0.0
                For Each c In ai_chidren
                    tempSum += Math.Abs(c.Eval)
                Next
                Dim tempList As New List(Of Double)(ai_chidren.Count)
                Dim newTempSum As Double = 0.0
                For i As Integer = 0 To ai_chidren.Count - 1
                    Dim temp = tempSum - ai_chidren(i).Eval
                    tempList.Add(temp)
                    newTempSum += temp
                Next
                'select
                Dim r = Me.m_rand.NextDouble()
                Dim cumulativeRatio As Double = 0.0
                For i As Integer = 0 To ai_chidren.Count - 1
                    cumulativeRatio += tempList(i) / newTempSum
                    If cumulativeRatio > r Then
                        Return i
                    End If
                Next
            Else
                Dim tempSum As Double = 0.0
                For Each c In ai_chidren
                    tempSum += c.Eval
                Next
                'select
                Dim r = Me.m_rand.NextDouble()
                Dim cumulativeRatio As Double = 0.0
                For i As Integer = 0 To ai_chidren.Count - 1
                    cumulativeRatio += ai_chidren(i).eval / tempSum
                    If cumulativeRatio > r Then
                        Return i
                    End If
                Next
            End If

            Return 0
        End Function

        ''' <summary>
        ''' using Elite Strategy
        ''' </summary>
        ''' <param name="ai_density">density</param>
        ''' <remarks>
        ''' Elite strategy
        ''' </remarks>
        Public Sub UseEliteStrategy(ByVal ai_density As Double)
            If ai_density > 1 Then
                Return
            End If
            If ai_density < 0 Then
                Return
            End If
            Dim index As Integer = CInt(Me.m_parents.Count * ai_density)
            If index = 0 Then
                Return
            End If

            'replace new point
            For i As Integer = index To Me.PopulationSize - 1
                Dim temp As New List(Of Double)
                For j As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                    If MyBase.InitialPosition IsNot Nothing AndAlso MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                        value += Me.InitialPosition(j)
                    End If
                    temp.Add(value)
                Next
                Me.m_parents(i) = New clsPoint(MyBase.m_func, temp)
            Next

            'iteration count reset
            Me.m_iteration = 0

            'reset error
            Me.m_error.Clear()
        End Sub

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result() As clsPoint
            Get
                Return Me.m_parents(0)
            End Get
        End Property

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
                Return Me.m_parents
            End Get
        End Property
#End Region

#Region "Private"
        ''' <summary>
        ''' Calc Triangle Area using Heron formula
        ''' </summary>
        ''' <param name="lengthA"></param>
        ''' <param name="lengthB"></param>
        ''' <param name="lengthC"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CalcTriangleArea(ByVal lengthA As Double, ByVal lengthB As Double, ByVal lengthC As Double) As Double
            '    p3
            '   /| 
            '  / |
            ' ----
            ' p1  p2
            Dim s = (lengthA + lengthB + lengthC) / 2.0
            Dim area = Math.Sqrt(s * (s - lengthA) * (s - lengthB) * (s - lengthC))
            Return area
        End Function

        ''' <summary>
        ''' UNDX CrossOver
        ''' </summary>
        ''' <param name="p1"></param>
        ''' <param name="p2"></param>
        ''' <param name="p3"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function UNDX(ByVal p1 As clsPoint, ByVal p2 As clsPoint, ByVal p3 As clsPoint) As List(Of clsPoint)
            'calc d
            Dim diffVectorP2P1 = p1 - p2
            Dim length = diffVectorP2P1.NormL2()
            Dim areaTriangle As Double = Me.CalcTriangleArea(length, (p3 - p2).NormL2(), (p3 - p1).NormL2())
            If length = 0 Then
                length += 0.0000000001 'avoid Zero Divide
            End If
            Dim d2 = 2.0 * areaTriangle / length 'S=1/2 * h * a -> h = 2.0 * S / a

            'If d2 < 0.0001 Then
            '    d2 += Me.m_rand.NextDouble()
            'End If

            'UNDX
            Dim children As New List(Of clsPoint)(Me.ChildrenSize)
            Dim g = (p1 + p2) / 2.0
            Dim sd1 = (Me.ALPHA * length) ^ 2
            Dim sd2 = (Me.BETA * d2 / Math.Sqrt(Me.m_func.NumberOfVariable)) ^ 2
            Dim e = diffVectorP2P1 / length
            Dim t = New clsEasyVector(Me.m_func.NumberOfVariable)
            For genChild As Integer = 0 To CInt(Me.ChildrenSize / 2 - 1)
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    t(i) = clsUtil.NormRand(0, sd2)
                Next
                t = t - (t.InnerProduct(e)) * e

                'child
                Dim child1(Me.m_func.NumberOfVariable - 1) As Double
                Dim child2(Me.m_func.NumberOfVariable - 1) As Double
                Dim ndRand = clsUtil.NormRand(0, sd1)
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    Dim temp = t(i) + ndRand * e(i)
                    child1(i) = g(i) + temp
                    child2(i) = g(i) - temp
                Next

                children.Add(New clsPoint(Me.m_func, child1))
                children.Add(New clsPoint(Me.m_func, child2))
            Next
            Return children
        End Function
#End Region
    End Class
End Namespace
