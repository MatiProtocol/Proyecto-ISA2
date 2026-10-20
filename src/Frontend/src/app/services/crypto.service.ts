import { Injectable } from '@angular/core';
import * as nacl from 'tweetnacl';

@Injectable({ providedIn: 'root' })
export class CryptoService {
  async generateKeyPair(): Promise<{ publicKeyBase64: string; privateKeyBase64: string }> {
    const keyPair = nacl.sign.keyPair();
    return {
      publicKeyBase64: this.uint8ArrayToBase64(keyPair.publicKey),
      privateKeyBase64: this.uint8ArrayToBase64(keyPair.secretKey),
    };
  }

  signChallenge(challenge: string, privateKeyBase64: string): string {
    const privateKey = this.base64ToUint8Array(privateKeyBase64);
    const challengeBytes = new TextEncoder().encode(challenge);
    const signature = nacl.sign.detached(challengeBytes, privateKey);

    return this.uint8ArrayToBase64(signature);
  }

  private base64ToUint8Array(base64: string): Uint8Array {
    const binary = atob(base64);
    const bytes = new Uint8Array(binary.length);
    for (let i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
    return bytes;
  }

  private uint8ArrayToBase64(arr: Uint8Array): string {
    return btoa(String.fromCharCode(...arr));
  }
}
