Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Differential Evolution Algorithm (DE/rand/1/bin)
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -similar to GA algorithm.
    ''' 
    ''' Memo:
    '''  Notation of DE
    '''   DE/x/y/z
    '''    x: pick parent strategy(rand or best)
    '''    y: number of difference vector
    '''    z: crossover scheme
    '''       ex.Binomial -> bin
    ''' 
    ''' Refference:
    ''' [1]Storn, R., Price, K., "Differential Evolution – A Simple and Efficient Heuristic for Global Optimization over Continuous Spaces", Journal of Global Optimization 11: 341–359.
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptDE : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' epsilon(Default:1e-8) for Criterion
        ''' </summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        ''' <summary>
        ''' Max iteration count
        ''' </summary>
        Public Property Iteration As Integer = 50000

        ''' <summary>
        ''' Range of initial value
        ''' </summary>
        Public Property InitialValueRange As Double = 5 'parameter range

        '----------------------------------------------------------------
        'DE parameters
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:100)
        ''' </summary>
        Public Property PopulationSize As Integer = 100

        ''' <summary>
        ''' Differential weight(Mutation ratio)(Default:0.5)
        ''' </summary>
        Public Property F As Double = 0.5

        ''' <summary>
        ''' Cross over ratio(Default:0.9)
        ''' </summary>
        Public Property CrossOverRatio As Double = 0.9

        'population
        Private m_parents As New List(Of clsPoint)

        'ErrorManage
        Private m_error As New clsError
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
                        temp.Add(Math.Abs(2.0 * InitialValueRange) * m_rand.NextDouble() - InitialValueRange)
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

                'DE
                For i As Integer = 0 To Me.PopulationSize - 1
                    'pick different parent without i
                    Dim randIndex As List(Of Integer) = clsUtil.RandomPermutaion(Me.m_parents.Count, i)
                    Dim xi = Me.m_parents(i)
                    Dim p1 As clsPoint = Me.m_parents(randIndex(0))
                    Dim p2 As clsPoint = Me.m_parents(randIndex(1))
                    Dim p3 As clsPoint = Me.m_parents(randIndex(2))

                    'Mutation and Crossover
                    Dim child = New clsPoint(Me.m_func)
                    Dim j = Me.m_rand.Next() Mod Me.m_func.NumberOfVariable
                    Dim D = Me.m_func.NumberOfVariable - 1
                    For k = 0 To Me.m_func.NumberOfVariable - 1
                        If Me.m_rand.NextDouble() < Me.CrossOverRatio OrElse k = D Then
                            child(j) = p1(j) + Me.F * (p2(j) - p3(j))
                        Else
                            child(j) = xi(k)
                        End If
                        j = (j + 1) Mod Me.m_func.NumberOfVariable 'next
                    Next
                    child.ReEvaluate()

                    'replace
                    If child.Eval < xi.Eval Then
                        Me.m_parents(i) = child
                    End If
                Next
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
        Public Overrides ReadOnly Property ResultForDebug As List(Of clsPoint)
            Get
                Return Me.m_parents
            End Get
        End Property
#End Region

#Region "Private"
       
#End Region
    End Class
End Namespace
