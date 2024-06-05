// See https://aka.ms/new-console-template for more informatin
using System;
using System.Linq;

// Matrix<float> matrix = new Matrix<float>(new float[,] {
//     {2.0f,62.0f, 0.0f,},
//     {0.0f,0.0f, 0.0f },
//     {0.0f,0.0f, 0.0f }
// });

// Matrix<float> matrix2 = new Matrix<float>(new float[,] {
//     {12.0f,3.0f, 0.0f,},
//     {0.0f,3.0f, 0.0f },
//     {0.0f,0.0f, 1.0f }
// });

// Matrix<float> c = Matrix<float>.Multiply(matrix, matrix2);
// c.Print();
Matrix<float> M = new Matrix<float>(new float[,] {
           /*q*//*z0*//*z1*//*z2*/  //z3 z4
   /*w0*/ {2.0f,      0.0f, 0.0f, 1.0f, -1.0f,1.0f},
   /*w1*/ { -1.0f,    0.0f, 0.0f, 1.0f, -2.0f,1.0f},
   /*w2*/ {3.0f,     -1.0f, -1.0f, 0.0f, 0.0f,1.0f},
   /*w3*/ {-2.0f,    1.0f, 2.0f, 0.0f, 0.0f,1.0f},
});
// w1 = 1q + 2 *z0 + 3*z1 + 5z2
// z0 =  -1q/2 - w1/2  - 3/2 z1 - 5/2 z2
// wo = [q,z0,], - > [q,0,0,0,0]
//z0,w1
var w0 = new EqVariable();
w0.row_index = 0;
w0.initial_index = 0;
w0.is_basic = true;
var w1 = new EqVariable();
w1.row_index = 1;
w1.initial_index = 1;
w1.is_basic = true;


var w2 = new EqVariable();
w2.row_index = 2;
w2.initial_index = 2;
w2.is_basic = true;

var w3 = new EqVariable();
w3.row_index = 3;
w3.initial_index = 3;
w3.is_basic = true;


var z0 = new EqVariable();
z0.row_index = 0;
z0.variable_type = "z";
z0.col_index = 1;
z0.initial_index = 0;
z0.is_basic = false;


var z1 = new EqVariable();
z1.row_index = 0;
z1.variable_type = "z";
z1.col_index = 2;
z1.initial_index = 1;
z1.is_basic = false;

var z2 = new EqVariable();
z2.row_index = 0;
z2.variable_type = "z";
z2.col_index = 3;
z2.initial_index = 2;
z2.is_basic = false;

var z3 = new EqVariable();
z3.row_index = 0;
z3.variable_type = "z";
z3.col_index = 4;
z3.initial_index = 3;
z3.is_basic = false;


var z4 = new EqVariable();
z4.row_index = 0;
z4.variable_type = "z";
z4.col_index = 5;
z4.initial_index = 4;
z4.is_basic = false;
EqVariable[] basics = { w0, w1, w2, w3 };
EqVariable[] nonBasics = { z0, z1, z2, z3 };
M.SetBasic(basics);
M.SetNonBasic(nonBasics);
M.Swap(z4, w3);
M.Print();
// w1 -> z0 ()
//zo = 
public class Matrix<T> where T : struct,
          IComparable,
          IComparable<T>,
          IConvertible,
          IEquatable<T>,
          IFormattable
{
    public T[,] data;
    EqVariable[] basics = { };
    EqVariable[] nonBasics = { };
    public Matrix(T[,] data)
    {
        this.data = data;
    }
    public void SetBasic(EqVariable[] basics) {
        this.basics = basics;
    }
    public void SetNonBasic(EqVariable[] nonBasics)
    {
        this.nonBasics = nonBasics;
    }
    public void Swap(EqVariable nonBasic, EqVariable basic)
    {
        //z0, w1 zo ->get column index okey w1 get row index
        // matrix[w1[rowIndex]] = [-q/z0.coeff, , - 1/ z.coeff, ];
        var rowIndex = basic.row_index;
        var colIndex = nonBasic.col_index;
        dynamic zCoeff = data[rowIndex, colIndex];
        for (int j = 0; j < data.GetLength(1); j++)
        {
            dynamic x = data[rowIndex, j];
            x = -x / zCoeff;
            if (j == colIndex)
            {
                data[rowIndex, j] =  1 / zCoeff;
            }
            else
            {
                data[rowIndex, j] = (T)x;
            }
        }
//#region swap copy w to z
        var basicCopy = new EqVariable();
        basicCopy.is_basic = basic.is_basic;
        basicCopy.row_index = basic.row_index;
        basicCopy.col_index = basic.col_index;

        var nonBasicCopy = new EqVariable();
        nonBasicCopy.is_basic = nonBasic.is_basic;
        nonBasicCopy.row_index = nonBasic.row_index;
        nonBasicCopy.col_index = nonBasic.col_index;

        basic.row_index = nonBasicCopy.row_index;
        basic.col_index = nonBasicCopy.col_index;
        basic.is_basic = false;


        nonBasic.row_index = basicCopy.row_index;
        nonBasic.col_index = basicCopy.col_index;
        nonBasic.is_basic = true;
        // nonBasics 
        nonBasics = nonBasics.Where(x => x != nonBasic).ToArray();
        basics = basics.Where(x => x != basic).ToArray();
        nonBasics.Append(basic);
        basics.Append(nonBasic);
//#endregion 
    for(int i=0; i < data.GetLength(0);i++){
        //
        if(i==rowIndex){
                continue;
        }
            dynamic _zCoeff = data[i, colIndex];
            for(int k =0;k<data.GetLength(1);k++){
                dynamic a; 
                if(k==colIndex){
                    a = default(T);
                } else {
                    a = data[i, k];
                }

                dynamic b = _zCoeff * data[rowIndex, k];
                data[i, k] = (T)a + b;
            }
            // dynamic currList = data[i];
    }
    }
    public void Print()
    {
        var rowCount = data.GetLength(0);
        var colCount = data.GetLength(1);
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
                Console.Write(String.Format("{0}\t", data[row, col]));
            Console.WriteLine();
        }
    }
    public static Matrix<T> Multiply(Matrix<T> A, Matrix<T> B)
    {
        int rA = A.data.GetLength(0);
        int cA = A.data.GetLength(1);
        int rB = B.data.GetLength(0);
        int cB = B.data.GetLength(1);

        T[,] res = new T[rA, cB];
        if (cA != rB)
        {
            Console.WriteLine("Matrixes can't be multiplied!!");
        }
        else
        {
            T temp = default(T);

            for (int i = 0; i < rA; i++)
            {
                for (int j = 0; j < cB; j++)
                {
                    temp = default(T);
                    for (int k = 0; k < cA; k++)
                    {
                        dynamic x = A.data[i, k];
                        dynamic y = B.data[k, j];

                        temp += (T)x * y;
                    }
                    res[i, j] = temp;
                }
            }
        }
        Matrix<T> C = new Matrix<T>(res);
        return C;
    }
}

public class EqVariable
{
    public bool is_basic;
    // w or z
    public string variable_type = "w";
    public int initial_index = 0;
    public int col_index = 0;
    public int row_index = 0;
    //  public bool i      
}
//     q, z0, z1, z2, z3
/* w0   1  2  3  4  5 7
   w1  -1  3  5  6  7 8      
   w2  -1  3  5  6  7 8      
   w3  -1  3  5  6  7 8      
*/
//not echanges