using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ArcsoftFaceTest
{
    public class AmFaceVerify
    {
        /**
        * 初始化人脸检测引擎
        * @return 初始化人脸检测引擎
        */
        [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_InitialFaceEngine(string appId, string sdkKey, IntPtr pMem, int lMemSize, ref IntPtr pEngine, int iOrientPriority, int nScale, int nMaxFaceNum);

        /**
        * 获取人脸检测 SDK 版本信息
        * @return 获取人脸检测SDK 版本信息
        */
        [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AFD_FSDK_GetVersion(IntPtr pEngine);

        /**
        * 根据输入的图像检测出人脸位置，一般用于静态图像检测
        * @return 人脸位置
        */
        [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_StillImageFaceDetection(IntPtr pEngine, IntPtr offline, ref IntPtr faceRes);


        /**
        * 初始化人脸识别引擎
        * @return 初始化人脸识别引擎
        */
        [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFR_FSDK_InitialEngine(string appId, string sdkKey, IntPtr pMem, int lMemSize, ref IntPtr pEngine);

        /**
        * 获取人脸识别SDK 版本信息
        * @return 获取人脸识别SDK 版本信息
        */
        [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr AFR_FSDK_GetVersion(IntPtr pEngine);

        /**
        * 提取人脸特征
        * @return 提取人脸特征
        */
        [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFR_FSDK_ExtractFRFeature(IntPtr pEngine, IntPtr offline, IntPtr faceResult, IntPtr localFaceModels);

        /**
        * 获取相似度
        * @return 获取相似度
        */
        [DllImport("libarcsoft_fsdk_face_recognition.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFR_FSDK_FacePairMatching(IntPtr pEngine, IntPtr faceModels1, IntPtr faceModels2, ref float fSimilScore);

        #region delete
        ///**
        // *  创建人脸检测引擎
        // *  @param [in] model_path 模型文件夹路径
        // *  @param [out] engine 创建的人脸检测引擎
        // *  @return =0 表示成功，<0 表示错误码。
        // */
        //[DllImport("AmFaceDet.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int AmCreateFaceDetectEngine(string modelPath, ref IntPtr faceDetectEngine);

        ///**
        // *  创建人脸识别引擎
        // *  @param [in] model_path 模型文件夹路径
        // *  @param [out] engine 创建的人脸识别引擎
        // *  @return =0 表示成功，<0 表示错误码。
        // */
        //[DllImport("AmFaceRec.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int AmCreateFaceRecogniseEngine(string modelPath, ref IntPtr facRecogniseeEngine);

        ///**
        // *  创建人脸比对别引擎
        // *  @param [in] model_path 模型文件夹路径
        // *  @param [out] engine 创建的人脸比对引擎
        // *  @return =0 表示成功，<0 表示错误码。
        // */
        //[DllImport("AmFaceCompare.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int AmCreateFaceCompareEngine(ref IntPtr facCompareEngine);

        ///**
        // *  设置人脸引擎参数
        // *  @param [in] engine 人脸引擎
        // *  @param [in] param 人脸参数
        // */
        //[DllImport("AmFaceDet.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void AmSetParam(IntPtr faceDetectEngine, [MarshalAs(UnmanagedType.LPArray)] [In] TFaceParams[] setFaceParams);

        ///**
        // * 人脸检测
        // * @param [in] engine 人脸引擎
        // * @param [in] bgr 图像数据，BGR格式
        // * @param [in] width 图像宽度
        // * @param [in] height 图像高度
        // * @param [in] pitch 图像数据行字节数
        // * @param [in,out] faces 人脸结构体数组，元素个数应等于期望检测人脸个数
        // * @param [in] face_count 期望检测人脸个数
        // * @return >=0 表示实际检测到的人脸数量，<0 表示错误码。
        // */
        //[DllImport("AmFaceDet.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int AmDetectFaces(IntPtr faceDetectEngine, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] image, int width, int height, int pitch, [MarshalAs(UnmanagedType.LPArray)] [In][Out] TAmFace[] faces, int face_count);

        ///**
        // * 抽取人脸特征
        // * @param [in] engine 人脸引擎
        // * @param [in] bgr 图像数据，BGR格式
        // * @param [in] width 图像宽度
        // * @param [in] height 图像高度
        // * @param [in] pitch 图像数据行字节数
        // * @param [in] face 人脸结构体
        // * @param [out] feature 人脸特征
        // * @return =0 表示成功，<0 表示错误码。
        // */
        //[DllImport("AmFaceRec.dll", CallingConvention = CallingConvention.Cdecl)]
        ////public static extern int AmExtractFeature(IntPtr faceEngine, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] image, int width, int height, int pitch, [MarshalAs(UnmanagedType.LPArray)] [In] TAmFace[] faces, ref byte[] feature);
        //public static extern int AmExtractFeature(IntPtr facRecogniseeEngine, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] image, int width, int height, int pitch, [MarshalAs(UnmanagedType.LPArray)] [In] TAmFace[] faces, [MarshalAs(UnmanagedType.LPArray)] [Out] byte[] feature);

        ///**
        // * 比对两个人脸特征相似度
        // * @param [in] engine 人脸引擎
        // * @param [in] feature1 人脸特征1
        // * @param [in] feature2 人脸特征2
        // * @return 人脸相似度
        // */
        //[DllImport("AmFaceCompare.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern float AmCompare(IntPtr facCompareEngine, byte[] feature1, byte[] feature2);
        #endregion
    }
}
