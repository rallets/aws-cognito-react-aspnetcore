import { IResponse } from "./IResponse";
import * as HttpStatus from "http-status-codes";

export class OkResponse<T> implements IResponse {
  public constructor(public body: T) {
  }
  headers = { "Content-Type": "application/json" };
  status = HttpStatus.StatusCodes.OK;
}
