export class ChallengeRequest {
  challenge = '';
  signature = '';

  constructor(challenge: string, signature: string) {
    this.challenge = challenge;
    this.signature = signature;
  }
}

export interface ChallengeResponse {
  challenge: string;
}
