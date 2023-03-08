package com.vuthao.bpmop.base;

import androidx.annotation.NonNull;

import org.bouncycastle.crypto.AsymmetricBlockCipher;
import org.bouncycastle.crypto.encodings.PKCS1Encoding;
import org.bouncycastle.crypto.engines.RSAEngine;
import org.bouncycastle.crypto.params.RSAKeyParameters;
import org.bouncycastle.crypto.util.PublicKeyFactory;
import org.bouncycastle.util.encoders.Base64;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.List;

public class Crypter {
    private static final String KEY = "MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAIL+WHEn6D91a7iOrD/iFog7OXU9j6SYLkOVwMfB86A6lXGONCtwpof9BzCtPfMFLbF/tJJlEv5EesDfAyTB5V8CAwEAAQ==";
    private RSAKeyParameters rsaKeyParameters;

    public Crypter() {
        byte[] keyInfoData = Base64.decode(KEY);
        try {
            rsaKeyParameters = (RSAKeyParameters) PublicKeyFactory.createKey(keyInfoData);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public String encrypt(String obj) {
        try {
            byte[] payloadBytes = obj.getBytes(StandardCharsets.UTF_8);
            AsymmetricBlockCipher cipher = getAsymmetricBlockCipher(true);
            byte[] encrypted = process(cipher, payloadBytes);

            return Base64.toBase64String(encrypted);
        } catch (Exception e) {
            e.printStackTrace();
        }

        return null;
    }

    @NonNull
    private AsymmetricBlockCipher getAsymmetricBlockCipher(boolean forEncryption) {
        PKCS1Encoding cipher = new PKCS1Encoding(new RSAEngine());
        cipher.init(forEncryption, rsaKeyParameters);
        return cipher;
    }

    private byte[] process(AsymmetricBlockCipher cipher, byte[] payloadBytes) {
        try {
            int length = payloadBytes.length;
            int blockSize = cipher.getInputBlockSize();

            List<Byte> plainTextBytes = new ArrayList<>();
            for (int chunkPosition = 0; chunkPosition < length; chunkPosition += blockSize) {
                int chunkSize = Math.min(blockSize, length - chunkPosition);
                byte[] bockGet = cipher.processBlock(payloadBytes, chunkPosition, chunkSize);
                for (byte b : bockGet) {
                    plainTextBytes.add(b);
                }
            }

            byte[] bytes = new byte[plainTextBytes.size()];
            for (int i = 0; i < bytes.length; i++) {
                bytes[i] = plainTextBytes.get(i);
            }

            return bytes;
        } catch (Exception e) {
            e.printStackTrace();
        }

        return null;
    }
}
