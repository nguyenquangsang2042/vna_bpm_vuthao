using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Biometrics;
using Android.OS;
using Android.Runtime;
using Android.Security.Keystore;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Security;
using Javax.Crypto;

namespace BPMOPMobile.Droid.Core.Class
{
    public class CustomBiometricHelper
    {
        private static readonly string KEY_NAME = Application.Context.PackageName;

        // We always use this keystore on Android.
        private static readonly string KEYSTORE_NAME = "AndroidKeyStore";

        // Should be no need to change these values.
        private static readonly string KEY_ALGORITHM = KeyProperties.KeyAlgorithmAes;
        private static readonly string BLOCK_MODE = KeyProperties.BlockModeCbc;
        private static readonly string ENCRYPTION_PADDING = KeyProperties.EncryptionPaddingPkcs7;
        private static readonly string TRANSFORMATION = KEY_ALGORITHM + "/" +
                                                BLOCK_MODE + "/" +
                                                ENCRYPTION_PADDING;
        readonly KeyStore _keystore;

        public CustomBiometricHelper()
        {
            _keystore = KeyStore.GetInstance(KEYSTORE_NAME);
            _keystore.Load(null);
        }


        /// <summary>
        /// Kiểm tra phiên bản Android đang sử dụng có hỗ trợ sinh trắc học hay không
        /// </summary>
        /// <returns></returns>
        public bool IsBiometricPromptEnabled()
        {
            return Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.P;
        }

        /// <summary>
        /// Kiểm tra thiết bị đang sử dụng có hỗ trợ dấu tay hay không
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsHardwareSupported(Context context)
        {
            var fingerprintManager = FingerprintManagerCompat.From(context);
            return fingerprintManager.IsHardwareDetected;
        }

        /// <summary>
        /// Kiểm tra số lượng vân tay hệ thống
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool IsFingerprintAvailable(Context context)
        {
            var fingerprintManager = FingerprintManagerCompat.From(context);
            return fingerprintManager.HasEnrolledFingerprints;
        }

        /// <summary>
        /// Khởi tạo builder BiometricPrompt
        /// </summary>
        /// <param name="_activity"></param>
        /// <param name="_context"></param>
        /// <param name="_negativeListener"></param>
        /// <param name="_title"></param>
        /// <param name="_description"></param>
        /// <param name="_negativeTitle"></param>
        /// <returns></returns>
        public BiometricPrompt.Builder CreateBiometricPromptBuilder(Activity _activity, Context _context, IDialogInterfaceOnClickListener _negativeListener,
            string _title = "" ,string _description ="", string _negativeTitle = "")
        {
            BiometricPrompt.Builder _biometric = null;
            try
            {
                _biometric = new BiometricPrompt.Builder(_context)
                    .SetTitle(!string.IsNullOrEmpty(_title) ? _title : "Xác thực dấu vân tay")
                    .SetDescription(!string.IsNullOrEmpty(_description) ? _description : "Quét dấu vân tay để đăng nhập vào BPM")
                    .SetNegativeButton(!string.IsNullOrEmpty(_negativeTitle) ? _negativeTitle : "Hủy", _activity.MainExecutor, _negativeListener);
            }
            catch (System.Exception ex)
            {
                // Do some thing when error
            }
            return _biometric;
        }

        #region Crypto Function

        public BiometricPrompt.CryptoObject BuildCryptoObject()
        {
            Cipher cipher = CreateCipher();
            return new BiometricPrompt.CryptoObject(cipher);
        }

        public Cipher CreateCipher(bool retry = true)
        {
            IKey key = GetKey();
            Cipher cipher = Cipher.GetInstance(TRANSFORMATION);
            try
            {
                cipher.Init(CipherMode.EncryptMode, key);
            }
            catch (KeyPermanentlyInvalidatedException e)
            {
                _keystore.DeleteEntry(KEY_NAME);
                if (retry)
                {
                    CreateCipher(false);
                }
                else
                {
                    throw new System.Exception("Could not create the cipher for fingerprint authentication.", e);
                }
            }
            return cipher;
        }

        public IKey GetKey()
        {
            IKey secretKey;
            if (!_keystore.IsKeyEntry(KEY_NAME))
            {
                CreateKey();
            }

            secretKey = _keystore.GetKey(KEY_NAME, null);
            return secretKey;
        }

        public void CreateKey()
        {
            KeyGenerator keyGen = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KEYSTORE_NAME);
            KeyGenParameterSpec keyGenSpec =
                new KeyGenParameterSpec.Builder(KEY_NAME, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(BLOCK_MODE)
                    .SetEncryptionPaddings(ENCRYPTION_PADDING)
                    .SetUserAuthenticationRequired(true)
                    .Build();
            keyGen.Init(keyGenSpec);
            keyGen.GenerateKey();
        }

        #endregion

        public class CustomBiometricAuthenticationCallback : BiometricPrompt.AuthenticationCallback
        {
            public Action<BiometricPrompt.AuthenticationResult> Success;
            public Action Failed;
            public Action<BiometricErrorCode, ICharSequence> Error;
            public Action<BiometricAcquiredStatus, ICharSequence> Help;

            /// <summary>
            /// Trigger khi authen được
            /// </summary>
            /// <param name="result"></param>
            public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result)
            {
                base.OnAuthenticationSucceeded(result);
                Success(result);
            }

            /// <summary>
            /// Trigger khi gặp lỗi không fix được
            /// </summary>
            /// <param name="result"></param>
            public override void OnAuthenticationError([GeneratedEnum] BiometricErrorCode errorCode, ICharSequence errString)
            {
                base.OnAuthenticationError(errorCode, errString);
                Error(errorCode, errString);
            }

            /// <summary>
            /// Trigger khi nhận dc finferprint nhưng ko đúng
            /// </summary>
            /// <param name="result"></param>
            public override void OnAuthenticationFailed()
            {
                base.OnAuthenticationFailed();
                Failed();
            }

            /// <summary>
            /// Trigger khi gặp lỗi fix được (bị dơ, mờ, ...)
            /// </summary>
            /// <param name="result"></param>
            public override void OnAuthenticationHelp([GeneratedEnum] BiometricAcquiredStatus helpCode, ICharSequence helpString)
            {
                base.OnAuthenticationHelp(helpCode, helpString);
                Help(helpCode, helpString);
            }
        }
    }
}